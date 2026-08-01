using System.Collections.Generic;
using Sims3.Gameplay.CAS;
using Sims3.UI;

namespace Destrospean.HairTrouble
{
    public static class Common
    {
        public static T GetRandomItem<T>(this IEnumerable<T> enumerable)
        {
            List<T> list = new List<T>(enumerable);
            return list[Sims3.Gameplay.Core.RandomUtil.GetInt(list.Count)];
        }

        public static void Notify(string message, SimDescription simDescription, StyledNotification.NotificationStyle style)
        {
            Notify(message, simDescription, style, true);
        }

        public static void Notify(string message, SimDescription fakeSimDescription, StyledNotification.NotificationStyle style, bool checkForFake)
        {
            SimDescription simDescription = fakeSimDescription;
            if (simDescription == null)
            {
                StyledNotification.Show(new StyledNotification.Format(message, style));
                return;
            }
            if (checkForFake)
            {
                simDescription = SimDescription.Find(fakeSimDescription.SimDescriptionId);
                if (simDescription == null)
                {
                    StyledNotification.Show(new StyledNotification.Format(message, style));
                    return;
                }
            }
            if (simDescription.CreatedSim != null)
            {
                StyledNotification.Show(new StyledNotification.Format(message, Sims3.SimIFace.ObjectGuid.InvalidObjectGuid, simDescription.CreatedSim.ObjectId, style));
            }
            else
            {
                StyledNotification.Show(new StyledNotification.Format(message, style));
            }
        }

        /// <summary>This method was borrowed from Lazy Duchess' Mono Patcher</summary>
        public static void ReplaceMethod(System.Reflection.MethodInfo oldMethod, System.Reflection.MethodInfo newMethod)
        {
            byte[] replacementByteArray = new byte[40];
            System.Runtime.InteropServices.Marshal.Copy(newMethod.MethodHandle.Value, replacementByteArray, 0, 40);
            System.Runtime.InteropServices.Marshal.Copy(replacementByteArray, 0, oldMethod.MethodHandle.Value, 24);
            System.Runtime.InteropServices.Marshal.Copy(replacementByteArray, 28, new System.IntPtr(oldMethod.MethodHandle.Value.ToInt32() + 28), 12);
        }

        public static void TryGetRandomItem<T>(this IEnumerable<T> enumerable, out T item) where T : class
        {
            List<T> list = new List<T>(enumerable);
            item = list.Count == 0 ? null : list[Sims3.Gameplay.Core.RandomUtil.GetInt(list.Count)];
        }

        public static void TryGetRandomItem<T>(this IEnumerable<T> enumerable, out T? item) where T : struct
        {
            List<T> list = new List<T>(enumerable);
            item = list.Count == 0 ? (T?)null : list[Sims3.Gameplay.Core.RandomUtil.GetInt(list.Count)];
        }
    }
}
