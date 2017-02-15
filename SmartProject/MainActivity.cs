using Android.App;
using Android.Widget;
using Android.OS;
using System.Net.Sockets;
using System;
using System.Net;
using System.Threading;
using System.Text;
using System.Runtime.InteropServices;

namespace SmartProject
{
    [Activity(Label = "SmartProject", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        DataSota sota;
        static ulong _idUni;
        Socket sender;
        private string ipadres = "192.168.12.33";
        private uint fromId = 2001; //это ид приложения нужно сделать форму регистрации пользователя где он вводит я "Иванов" "КК" "Экипаж ТМА"

        protected override void OnCreate(Bundle bundle) 
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);
            //Xamarin.Forms.Forms.Init(this, bundle);
            //LoadApplication(new App());


            Button btn = FindViewById<Button>(Resource.Id.button1);
            btn.Click += btn_Click;


            sota = new DataSota();
              
            uint a = 7000;          //это id формата КТПС 
            uint b = fromId;           //это ид приложения нужно сделать форму регистрации пользователя где он вводит я "Иванов" "КК" "Экипаж ТМА"
            _idUni = (uint)(((ulong)((uint)(a) & 0xffffffff)) | ((uint)((ulong)((uint)(b) & 0xffffffff))) << 32); 

        }


        //Коннект
        private void btn_Click(object sender, System.EventArgs e)
        {
            try
            {

                #region welcome пакет
                char[] Text = new char[256];        //формируем welcome пакет
                char[] Text2 = new char[256];       //формируем welcome пакет
                string name = "Иванов Иван";        //формируем welcome пакет 
                string ip = ipadres;        //формируем welcome пакет

                //byte[] bytes = Encoding.Default.GetBytes(name);
                //string test = Encoding.UTF8.GetString(bytes);


                name.CopyTo(0, Text, 0, name.Length);  //формируем welcome пакет
                ip.CopyTo(0, Text2, 0, ip.Length);     //формируем welcome пакет 

                


                Text[255] = '0';                    //формируем welcome пакет
                Text2[255] = '0';                   //формируем welcome пакет
                DataSota.TsotaWelcomeq welcome = new DataSota.TsotaWelcomeq(_idUni, Text, Text2);
                #endregion

                #region шапка
                DataSota.TsotaPaket pac = new DataSota.TsotaPaket();
                pac.uni = long.Parse("8642106494615744221");
                pac.typemes = 20;
                pac.pack = 0;
                pac.size = 520;
                pac.packsize = 520;//Marshal.SizeOf(welcome);
                pac.fromind = fromId;
                #endregion

                byte[] arr = sota.StructToBytes(pac);           //переводим стуктуру шапки в последовательность байт
                byte[] arr2 = sota.StructToBytes2(welcome);     //переводим welcome

                Socket(25113, arr, arr2);
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
            }
            finally
            {
                //послать 21 сообщение и закрыть приложение
            }
        }

        //утанавливаем связь шлем welcome 
        public void Socket(int port, byte[] arr, byte[] arr2)
        {
            // Устанавливаем удаленную точку для сокета
            IPAddress ipAddr = IPAddress.Parse(ipadres);//ipHost.AddressList[0]; 
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, port);

            sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // Соединяем сокет с удаленной точкой
            sender.Connect(ipEndPoint);

            // Отправляем данные через сокет
            int bytesSent = sender.Send(arr);
            int bytesSent2 = sender.Send(arr2);

            //функция ожидает ответа от соты и повторяется рекурсией пока тип сообщения не будет 21
            Thread receiveThread = new Thread(new ThreadStart(SendMessageFromSocket));
            receiveThread.Start(); //старт потока
            //SendMessageFromSocket();

            //SendMessageFromSocket(sender);
        }

        //функция ожидает ответа от соты и повторяется рекурсией пока тип сообщения не будет 21 
        public void SendMessageFromSocket()
        {
            // Буфер для входящих данных
            byte[] bytes = new byte[600];

            // Получаем ответ от сервера 
            int bytesRec = sender.Receive(bytes);

            if (bytesRec >= 32)
            {
                DataSota.TsotaPaket paket = ByteArrayToNewStruct(bytes);
                if (paket.uni.ToString() != "8642106494615744221")  //пакет битый
                {

                }
                else
                {
                    if (bytesRec == paket.packsize + Marshal.SizeOf(typeof(DataSota.TsotaPaket)))     //пакет пришел полностью
                    {
                        int id = (int)_idUni;
                        if (paket.typemes == 20)
                        {

                        }
                        if (paket.typemes == id)
                        {

                            //var str = ByteArrayToNewStruct(bytes);  //функция перевода из массива байтов в структуру TsotaPaket 

                            byte[] byteMess = new byte[paket.packsize];         //создаем новый буфер
                            Array.Copy(bytes, 32, byteMess, 0, paket.packsize); //копируем оставшиеся байты 
                            var messAndroid = ByteArrayToNewStructMessage(byteMess);     //затем разбираем структуру сообщения


                            byte[] mess = new byte[256];
                            Array.Copy(bytes, 40, mess, 0, 256);


                            //char aa= BitConverter.ToChar(byteMess,32);
                            Encoding dec = Encoding.GetEncoding(1251);

                            char[] bb = new char[256];
                            int i = dec.GetChars(mess, 0, 256, bb, 0);
                            string asciiString = new string(bb);


                            //uia

                            Button btn = FindViewById<Button>(Resource.Id.button1);
                            btn.Text = "43333";

                        }

                    }
                    else   //пакет нужно дособирать
                    {

                    }

                    // Используем рекурсию для неоднократного вызова SendMessageFromSocket()  
                    if (paket.typemes != 21)
                        SendMessageFromSocket();
                }

            }

            // Освобождаем сокет
            sender.Shutdown(SocketShutdown.Both);
            sender.Close();
        }

        //функция перевода из массива байтов в структуру TsotaPaket 
        public static DataSota.TsotaPaket ByteArrayToNewStruct(byte[] bytes)
        {
            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            DataSota.TsotaPaket sotaPaket = (DataSota.TsotaPaket)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(DataSota.TsotaPaket));
            handle.Free();
            return sotaPaket;
        }

        public static DataSota.TSOTAKOSMOPARKMESAGEANDROID ByteArrayToNewStructMessage(byte[] bytes)
        {
            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);

            DataSota.TSOTAKOSMOPARKMESAGEANDROID sotaPaket = new DataSota.TSOTAKOSMOPARKMESAGEANDROID();
            //sotaPaket.Type = 0;
            //sotaPaket.Index = 1;
            //sotaPaket.Value[0] = 'a';
            sotaPaket = (DataSota.TSOTAKOSMOPARKMESAGEANDROID)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(DataSota.TSOTAKOSMOPARKMESAGEANDROID));
            handle.Free();
            return sotaPaket;
        }
    }
}

