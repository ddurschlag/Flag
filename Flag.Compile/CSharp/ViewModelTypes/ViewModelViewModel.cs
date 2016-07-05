using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flag.Compile.CSharp.ViewModelTypes
{
    public class ViewModelViewModel
    {
        public ViewModelViewModel(ViewModelType viewModel)
        {
            new Assigner(this).Visit(viewModel);
        }

        public StringViewModel String { get; private set; }
        public ListViewModel List { get; private set; }
        public PurePropertyViewModel PureProperty { get; private set; }
        public MultiLoopViewModel MultiLoop { get; private set; }
        public ComplexViewModel Complex { get; private set; }
        public LabelViewModel Label { get; private set; }

        private class Assigner : ViewModelTypeVisitor
        {
            public Assigner(ViewModelViewModel me)
            {
                Me = me;
            }

            private ViewModelViewModel Me;

            public override void Visit(MultiLoopViewModel m)
            {
                Me.MultiLoop = m;
            }

            public override void Visit(ComplexViewModel m)
            {
                Me.Complex = m;
            }

            public override void Visit(PurePropertyViewModel m)
            {
                Me.PureProperty = m;
            }

            public override void Visit(ListViewModel m)
            {
                Me.List = m;
            }

            public override void Visit(StringViewModel m)
            {
                Me.String = m;
            }

            public override void Visit(LabelViewModel m)
            {
                Me.Label = m;
            }
        }
    }
}
