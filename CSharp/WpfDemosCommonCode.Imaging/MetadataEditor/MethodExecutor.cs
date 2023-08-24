using System.Reflection;

namespace WpfDemosCommonCode.Imaging
{
    class MethodExecutor
    {

        #region Fields

        object _obj;
        MethodInfo _method;

        #endregion



        #region Constructor

        public MethodExecutor(object obj, MethodInfo method)
        {
            _method = method;
            _obj = obj;
        }

        #endregion

        
        
        #region Methods
        
        public void Execute()
        {
            _method.Invoke(_obj, new object[] { });
        }

        public override string ToString()
        {
            return _method.Name;
        }

        #endregion

    }
}

