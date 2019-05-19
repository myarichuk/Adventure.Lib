using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;

namespace Adventure.Map
{
    public class Map<TData> : IDisposable where TData : struct
    {
        private const int AccessorViewCacheSize = 1024;

        private readonly MemoryMappedFile _mmf;
        private readonly ViewAccessorCache _cache;
        private readonly int _cellSize;
        public long Width { get; }
        public long Height { get; }                

        public Map(string name, long width, long height)
        {
            Width = width;
            Height = height;
            _cellSize = Marshal.SizeOf(typeof(TData));
            _mmf = MemoryMappedFile.CreateOrOpen(name, width * height * _cellSize);
            _cache = new ViewAccessorCache(AccessorViewCacheSize, this);            
        }

        public Stream Export()
        {
            var viewStream = _mmf.CreateViewStream(0, Width * Height * _cellSize);
            return viewStream;
        }

        public void Import(Stream stream)
        {
            using (var dataStream = _mmf.CreateViewStream(0, Width * Height * _cellSize)) 
                stream.CopyTo(dataStream);
        }

        // ReSharper disable once UnusedMember.Global
        public TData this[long x, long y]
        {
            get => Get(x, y);
            set => Put(x, y, value);
        }

        public TData Get(long x, long y)
        {
            VerifyBoundaries(x, y);

            var accessor = _cache.GetFor(x, y);
            accessor.Read(0,out TData cellData);

            return cellData;
        }

        public void Put(long x, long y, TData data)
        {
            VerifyBoundaries(x, y);

            var accessor = _cache.GetFor(x, y);

            accessor.Write(0,ref data);
            accessor.Flush();
        }

        private void VerifyBoundaries(long x, long y)
        {
            if (x < 0 || x > Width)
                throw new ArgumentOutOfRangeException(nameof(x));
            if (y < 0 || y > Height)
                throw new ArgumentOutOfRangeException(nameof(y));
        }

        private class ViewAccessorCache
        {
            private readonly Dictionary<(long,long), MemoryMappedViewAccessor> _cache = new Dictionary<(long, long), MemoryMappedViewAccessor>();
            private readonly int _maxRetainCount;
            private readonly Map<TData> _parent;

            public ViewAccessorCache(int maxRetainCount, Map<TData> parent)
            {
                _maxRetainCount = maxRetainCount;
                _parent = parent;
            }

            public MemoryMappedViewAccessor GetFor(long x, long y)
            {
                var key = (x, y);
                if (_cache.Count < _maxRetainCount)
                    return _cache.TryGetValue(key, out var accessor) ? accessor : CreateAndCacheAccessor(key);

                _cache.Clear();
                return CreateAndCacheAccessor(key);

                MemoryMappedViewAccessor CreateAndCacheAccessor((long x, long y) cacheKey)
                {
                    var accessor = _parent._mmf.CreateViewAccessor(cacheKey.x * _parent.Width + cacheKey.y, _parent._cellSize);
                    _cache.Add(cacheKey, accessor);
                    return accessor;
                }
            }
        }

        public void Dispose()
        {
            _mmf.Dispose();
        }
    }
}
