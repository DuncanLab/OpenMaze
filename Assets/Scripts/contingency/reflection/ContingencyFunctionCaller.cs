using System.Reflection;
using data;
using trial;
using UnityEngine;

namespace contingency.reflection
{
    public class ContingencyFunctionCaller : IContingencyFunctionCaller
    {

        public static ContingencyFunctionCaller Create()
        {
            return new ContingencyFunctionCaller();
        }
        
        private ContingencyFunctionCaller(){}
        
        /**
         * Uses reflection to call the contingency function and get the result.
         *
         * See main.ContingencyFunctions
         */
        public string InvokeContingencyFunction(TrialProgress tp, Data.Contingency contingency)
        {
            var contingencyFunction = contingency.ContingencyFunction;
            var functionInput = contingency.FunctionInput;

            var func =
                typeof(ContingencyFunctions).GetMethod(contingencyFunction, BindingFlags.Static | BindingFlags.Public);

            if (func == null)
            {
                Debug.LogError($"Contingency Function {contingencyFunction} doesn't exist.");
                Application.Quit();
                return null;
            }

            var result = (string) func.Invoke(null, new object[] {tp, functionInput});
            Debug.Log($"Output from the Contingency Function is {result}");
            return result;
        }
    }
}