//Project: MjpegStreamer.UWP
//Filename: ActionItem.cs
//Version: 20170907

using System;
using System.Threading.Tasks;

namespace Chantzaras.Tasks
{

    static class ActionItem //see https://github.com/dstuckims/azure-relay-dotnet/commit/93777a9f8563bbdacc4b854afd9fb21a968196b9
    {
        public static Task Schedule(Action<object> action, object state, bool attachToParent = false)
        {
            // UWP doesn't support ThreadPool[.QueueUserWorkItem] so just use Task.Factory.StartNew
            return Task.Factory.StartNew(s => action(s), state, (attachToParent) ? TaskCreationOptions.AttachedToParent : TaskCreationOptions.DenyChildAttach);
        }
    }

}