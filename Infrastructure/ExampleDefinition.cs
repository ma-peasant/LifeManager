using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeManager.Infrastructure
{
    public  class ExampleDefinition
    {
        public ExampleDefinition(string name, Type control)
        {
            this.Name = name;
            this.Control = control;
        }
        public Type Control { get; }
        public string Name { get; }

        public override string ToString() => Name;
    }
}
