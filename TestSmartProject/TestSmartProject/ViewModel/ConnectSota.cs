using Android.App;
using Android.Widget;
using Android.OS;
using System.Net.Sockets;
using System;
using System.Net;
using System.Threading;
using System.Text;
using System.Runtime.InteropServices;
using TestSmartProject.Model;

namespace TestSmartProject.ViewModel
{
    public class ConnectSota
    {
        DataSota sota;
        Socket sender;

        static ulong _idUni;
        private string _ipadres = "192.168.12.33";
        private string _name;
        private int _port = 25113;
        private uint _fromId;

        public ConnectSota(int fromId, string IpAdress, string Name,ref ulong uni)
        {
            _ipadres = IpAdress;
            _name = Name;
            _fromId = (uint)fromId;

            sota = new DataSota();

            uint a = 7000;          //это id формата КТПС 
            uint b = (uint)fromId;           //это ид приложения нужно сделать форму регистрации пользователя где он вводит я "Иванов" "КК" "Экипаж ТМА"
            _idUni = (uint)(((ulong)((uint)(a) & 0xffffffff)) | ((uint)((ulong)((uint)(b) & 0xffffffff))) << 32);
            uni = _idUni;
        }

        public Socket SendWelcome()
        {
            try
            {

                #region welcome пакет
                char[] Text = new char[256];        //формируем welcome пакет
                char[] Text2 = new char[256];       //формируем welcome пакет
                //string name = "Иванов Иван";        //формируем welcome пакет 
                //string ip = ipadres;        //формируем welcome пакет

                //byte[] bytes = Encoding.Default.GetBytes(name);
                //string test = Encoding.UTF8.GetString(bytes);


                _name.CopyTo(0, Text, 0, _name.Length);  //формируем welcome пакет
                _ipadres.CopyTo(0, Text2, 0, _ipadres.Length);     //формируем welcome пакет 




                Text[255] = '0';                    //формируем welcome пакет
                Text2[255] = '0';                   //формируем welcome пакет
                DataSota.TsotaWelcome welcome = new DataSota.TsotaWelcome(_idUni, Text, Text2);
                #endregion

                #region шапка
                DataSota.TsotaPaket pac = new DataSota.TsotaPaket();
                pac.uni = long.Parse("8642106494615744221");
                pac.typemes = 20;
                pac.pack = 0;
                pac.size = 520;
                pac.packsize = 520;//Marshal.SizeOf(welcome);
                pac.fromind = _fromId;
                #endregion

                byte[] arr = sota.StructToBytes(pac);           //переводим стуктуру шапки в последовательность байт
                byte[] arr2 = sota.StructToBytes2(welcome);     //переводим welcome

                Socket(_port, arr, arr2);
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
            }
            finally
            {
                //послать 21 сообщение и закрыть приложение
            }
            return sender;
        }

        //утанавливаем связь шлем welcome 
        public void Socket(int port, byte[] arr, byte[] arr2)
        {
            // Устанавливаем удаленную точку для сокета
            IPAddress ipAddr = IPAddress.Parse(_ipadres);//ipHost.AddressList[0]; 
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, port);

            sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // Соединяем сокет с удаленной точкой
            sender.Connect(ipEndPoint);

            // Отправляем данные через сокет
            int bytesSent = sender.Send(arr);
            int bytesSent2 = sender.Send(arr2);

            

        }
    }
}
