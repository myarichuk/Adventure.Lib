using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Autofac;
using Entitas;
using Entitas.Utils;
using Fasterflect;

namespace Adventure.EntityUtils
{
    public class EntityFactory
    {
        private readonly IContainer _componentContainer;
        private readonly IContext _entityContext;

        public EntityFactory(IContext entityContext)
        {
            _entityContext = entityContext;
            var builder = new ContainerBuilder();

            var applicationAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            var pluginFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins");
            var pluginAssemblies = !Directory.Exists(pluginFolder) ? Enumerable.Empty<Assembly>() : Directory.GetFiles(pluginFolder, "*.dll", SearchOption.AllDirectories).Select(Assembly.LoadFrom);

            builder.RegisterAssemblyTypes(applicationAssemblies.Concat(pluginAssemblies).ToArray())
                .Where(t => t.ImplementsInterface<IComponent>())
                .AsImplementedInterfaces()
                .Named<IComponent>(t => t.Name)
                .AsSelf();

            _componentContainer = builder.Build();
        }

        public Entity CreateEntity(EntityTemplate template)
        {
            var entity = _entityContext.CreateEntity();

            if (!TryAssembleComponents(template, entity))
                return null;

            //TODO : handle hierarchy and inheritance here

            return entity;
        }

        private bool TryAssembleComponents(EntityTemplate template, Entity entity)
        {
            foreach (var component in template.Components)
            {
                var componentInstance = _componentContainer.ResolveNamed<IComponent>(component.Key);
                if (componentInstance == null)
                {
                    //TODO: add logging so if component name is wrong I can know about it
                    return false;
                }

                foreach (var prop in component.Value)
                {
                    //TODO: add logging so if setting property value fails, I can know about it...
                    componentInstance.TrySetValue(prop.Key, prop.Value);
                }

                var index = Array.IndexOf(entity.contextInfo.componentTypes, componentInstance.GetType());
                if (index == -1)
                {
                    var componentFoo = entity.CreateComponent(-1, componentInstance.GetType());
                }
                entity.AddComponent(index, componentInstance);
            }

            return true;
        }
    }
}
