using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using DS = data.DataSingleton;
//This is a standard block trial

using data;
using UnityEngine;
using Random = UnityEngine.Random;

namespace main
{
    
    public class Block
    {
        
        private readonly Data.BlockData _blockData;
        private Loader.LinkedListNode<Data.Trial> _head;
        private Loader.LinkedListNode<Data.Trial> _curr;




        private void Init()
        {
            _head = new Loader.LinkedListNode<Data.Trial>();

            var temp = _head;
            var cnt = 0;
            
            var tmplist = new List<int>(_blockData.RandomTrialType);
            
            foreach (var i in _blockData.TrialOrder)
            {
                if (i >= 0)
                    temp.Value = DS.GetData().TrialData[i];
                else
                {
                    var val = Random.Range(0, tmplist.Count);
                        
                    var val2 = tmplist[val];
                    
                                        
                    if (_blockData.Replacement == 0)
                    {
                        tmplist.Remove(val2);
                    }
                    var tempTrial =  DS.GetData().TrialData[val2];

                    if (tempTrial.HasRecursiveTrial == 1)
                    {
                        temp.Value = DS.GetData().TrialData[tempTrial.RecursiveTrialReference];
                        temp.Next = new Loader.LinkedListNode<Data.Trial>();
                        temp = temp.Next;
                        temp.Value = tempTrial;
                    }
                    
                }

                if (cnt++ != _blockData.TrialOrder.Count - 1)
                    temp.Next = new Loader.LinkedListNode<Data.Trial>();
                temp = temp.Next;

            }
            _curr = _head;

        }
        
        public Block(Data.BlockData blockData)
        {
            _blockData = blockData;
            Init();

        }

        public Data.Trial Peek()
        {
            return _curr.Value;
        }
        
        public Data.Trial Progress()
        {
            
            if (_blockData.EndFunction != null)
            {
                var tmp = _blockData.EndFunction;
                var func = typeof(Functions).GetMethod(tmp, BindingFlags.Static | BindingFlags.Public);
                var val = (bool) func.Invoke(null, new object[] {_blockData});
                if (val)
                {
                    return null;
                }


                if (IsDone())
                {
                    Init();
                    return _curr.Value;
                }
            }
            else
            {
                if (IsDone()) return null;
            }
            
            
            
            var returnValue = _curr.Value;
            _curr = _curr.Next;
            return returnValue;
            
        }
        
        
        
        private bool IsDone()
        {
            return _curr.Next == null;
        }


        public void Log()
        {
            Loader.LogData(_blockData.BlockName + ", " + _blockData.Notes);
            
        }
    }
}
