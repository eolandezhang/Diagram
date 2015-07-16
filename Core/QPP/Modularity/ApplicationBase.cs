using QPP.Context;
using QPP.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Modularity
{
    public abstract class ApplicationBase : IApplication
    {
        public ApplicationBase()
        {
            var ctx = new AppContext();
            ctx.AppTypeName = GetType().FullName;
            OriginalContext = ctx;
            Context = ctx;
        }

        public abstract void Initialize();

        public IAppContext Context { get; private set; }

        public IAppContext OriginalContext { get; private set; }

        public IAppContext CreateContext()
        {
            var ctx = new AppContext();
            ctx.AppTypeName = GetType().Name;
            ctx.Parent = Context;
            return ctx;
        }

        public void Simulate(IAppContext ctx)
        {
            if (ctx.AppTypeName != Context.AppTypeName)
                throw new RuntimeException("The AppTypeName of the AppContext is not match");
            Context = ctx;
        }

        public void Reset()
        {
            Context = OriginalContext;
        }

        public bool CanSerialize { get; set; }

        public void Serialize(Runtime.Serialization.SerializationInfo info)
        {
            OnSerialize(info);
        }

        public void Deserialize(Runtime.Serialization.SerializationInfo info)
        {
            OnDeserialize(info);
        }

        protected virtual void OnSerialize(Runtime.Serialization.SerializationInfo info)
        {

        }

        protected virtual void OnDeserialize(Runtime.Serialization.SerializationInfo info)
        {

        }
    }
}
