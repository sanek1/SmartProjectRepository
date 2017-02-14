using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Runtime.InteropServices;

namespace SmartProject
{
    public class DataSota
    {

        public struct TsotaPaket
        {
            public long uni;
            public int typemes;
            public int pack;
            public int size;
            public int packsize;
            public long fromind;
        }
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        public struct TsotaWelcomeq
        {
            public ulong idChanel;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public char[] name;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public char[] ip;

            public TsotaWelcomeq(ulong a, char[] b, char[] c)
            {
                idChanel = a;
                name = b;
                ip = c;
            }
        }
        private struct TSOTAKOSMOPARKNEXTTIPS
        {
            public int Index;
            public int Value;
        }
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        public class TSOTAKOSMOPARKMESAGEANDROID
        {
            public int Index;				//id узла
            public int Type;			//тип сообщения
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public char[] Value = new char[256];      //сообщение

            //public TSOTAKOSMOPARKMESAGEANDROID(int i)
            //{
            //    Index = 0;
            //    Type = 0;
            //    Value = new char[256];
            //}
            //public TSOTAKOSMOPARKMESAGEANDROID(int index, int type, string message)
            //{
            //    Index = index;
            //    Type = type;
            //    Message = message;
            //}
        };

        public byte[] StructToBytes(TsotaPaket myStruct)
        {
            //int size = Marshal.SizeOf(myStruct);
            int size = 8 + 4 + 4 + 4 + 4 + 8;
            byte[] arr = new byte[size];

            IntPtr buffer = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(myStruct, buffer, false);
            Marshal.Copy(buffer, arr, 0, size);
            Marshal.FreeHGlobal(buffer);

            return arr;
        }
        public byte[] StructToBytes2(TsotaWelcomeq myStruct2)
        {
            //int size = Marshal.SizeOf(myStruct);
            byte[] arr2 = new byte[1];
            arr2[0] = 0;

            int size = 256 + 256 + 8;
            byte[] arr = new byte[size];

            IntPtr buffer = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(myStruct2, buffer, true);

            Marshal.Copy(buffer, arr, 0, size);
            //Marshal.Copy(buffer+20,arr2, 0, 1);
            Marshal.FreeHGlobal(buffer);

            return arr;
        }
        private static byte[] StructToBytes3(TSOTAKOSMOPARKNEXTTIPS myStruct)
        {
            int size = Marshal.SizeOf(myStruct);
            byte[] arr = new byte[size];

            IntPtr buffer = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(myStruct, buffer, false);
            Marshal.Copy(buffer, arr, 0, size);
            Marshal.FreeHGlobal(buffer);

            return arr;
        }
        private static byte[] StructToBytes4(TSOTAKOSMOPARKMESAGEANDROID myStruct)
        {
            int size = Marshal.SizeOf(myStruct);
            byte[] arr = new byte[size];

            IntPtr buffer = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(myStruct, buffer, false);
            Marshal.Copy(buffer, arr, 0, size);
            Marshal.FreeHGlobal(buffer);

            return arr;
        }

        private static TsotaPaket ByteArrayToNewStuff(byte[] bytes)
        {
            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            TsotaPaket stuff = (TsotaPaket)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(TsotaPaket));
            handle.Free();
            return stuff;
            //int length = Marshal.SizeOf(structureObj); 
            //IntPtr ptr = Marshal.AllocHGlobal(length);
            //Marshal.Copy(bytearray, 0, ptr, length);
            //structureObj = Marshal.PtrToStructure(Marshal.UnsafeAddrOfPinnedArrayElement(bytearray, position), structureObj.GetType());
            //Marshal.FreeHGlobal(ptr);
            //return structureObj;
        }

        public static TSOTAKOSMOPARKMESAGEANDROID fromBytes(byte[] arr)
        {
            TSOTAKOSMOPARKMESAGEANDROID str = new TSOTAKOSMOPARKMESAGEANDROID();

            int size = Marshal.SizeOf(str);
            IntPtr ptr = Marshal.AllocHGlobal(size);

            Marshal.Copy(arr, 0, ptr, size);

            Marshal.PtrToStructure(ptr, str);
            Marshal.FreeHGlobal(ptr);

            return str;
        }

    }
}