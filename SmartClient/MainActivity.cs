using Android.App;
using Android.Widget;
using Android.OS;
using System.Runtime.InteropServices;
using System.Net.Sockets;
using System;
using System.Threading;
using System.Net;
using System.Text;
using System.Net.NetworkInformation;
using Android.Views;

namespace SmartClient
{
    [Activity(Label = "SmartClient", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        DataSota sota;
        static ulong _idUni;
        Socket sender;
        string inputdata = "";
        private const string ip = "192.168.12.79";
        private const int port = 25113;
        bool bComp1ON = false;
        bool bPo1ON = false;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);


            Button btn = FindViewById<Button>(Resource.Id.butConnect);
            btn.Click += btn_Click;

 

            sota = new DataSota();

            uint a = 7000;
            uint b = 500;
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
                pac.fromind = 2001;
                #endregion

                byte[] arr = sota.StructToBytes(pac);           //переводим стуктуру шапки в последовательность байт
                byte[] arr2 = sota.StructToBytes2(welcome);     //переводим welcome

                Socket(port, arr, arr2);


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

        private void btnnav_Click(object sender, EventArgs e)
        {
            SetContentView(Resource.Layout.layout1);
        }

        [Java.Interop.Export("butSet_Click")]
        public void butSet_Click(View v)
        {
            SetContentView(Resource.Layout.settings);
        }

        [Java.Interop.Export("butExit_Click")]
        public void butExit_Click(View v)
        {
            System.Environment.Exit(0);
        }

        [Java.Interop.Export("switchComp_Click")]

        public void switchComp_Click(View v)
        {

            DataSota.TSOTAKOSMOPARKMESAGEANDROID tsota = new DataSota.TSOTAKOSMOPARKMESAGEANDROID();

            DataSota.TsotaPaket pac = new DataSota.TsotaPaket();
            pac.uni = long.Parse("8642106494615744221");
            pac.typemes = 5101;
            pac.pack = 0;
            pac.size = Marshal.SizeOf(tsota);
            pac.packsize = Marshal.SizeOf(tsota);
            pac.fromind = 2001;
            tsota.Index = 702;
            tsota.Type = 3;

            byte[] arr = sota.StructToBytes(pac);
            byte[] arr2 = sota.StructToBytes4(tsota);

            // Устанавливаем удаленную точку для сокета
            IPAddress ipAddr = IPAddress.Parse(ip);
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, port);

            sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // Соединяем сокет с удаленной точкой
            sender.Connect(ipEndPoint);

            // Отправляем данные через сокет
            int bytesSent = sender.Send(arr);
            int bytesSent2 = sender.Send(arr2);

            ImageView qImageView = FindViewById<ImageView>(Resource.Id.imgComputer1);

            if (bComp1ON) qImageView.SetImageResource(Resource.Drawable.ComputerOff);
            else qImageView.SetImageResource(Resource.Drawable.ComputerOn);

            bComp1ON = !bComp1ON;

        }

        [Java.Interop.Export("switchPo_Click")]

        public void switchPo_Click(View v)
        {

            ImageView qImageView = FindViewById<ImageView>(Resource.Id.imgComputer1);

            if (bPo1ON) qImageView.SetImageResource(Resource.Drawable.ComputerOff);
            else qImageView.SetImageResource(Resource.Drawable.PoOn);

            bPo1ON = !bPo1ON;
        }


        [Java.Interop.Export("butBack_Click")]

        public void butBack_Click(View v)
        {
            SetContentView(Resource.Layout.Main);
        }

        [Java.Interop.Export("butNavigate")]

        public void butNavigate(View v)
        {
            SetContentView(Resource.Layout.layout1);
        }

        void CheckConnection()
        {
            int timeout = 100;
            Ping ping = new Ping();
            PingReply reply = ping.Send(ip, timeout);

            if (reply != null && reply.Status == IPStatus.Success)
                this.RunOnUiThread(() => {
                    Toast.MakeText(this.ApplicationContext, "Hello", ToastLength.Short).Show();

                });
            if (reply != null && reply.Status == IPStatus.TimedOut)
                this.RunOnUiThread(() => {
                    Toast.MakeText(this.ApplicationContext, "Failed connection! Time out!", ToastLength.Short).Show();
                });
        }

        //private void btn2_Click(object sender, System.EventArgs e)
        //{
        //    Button btn2 = FindViewById<Button>(Resource.Id.button2);
        //    btn2.Text = inputdata;
        //}


        //утанавливаем связь шлем welcome 
        public void Socket(int port, byte[] arr, byte[] arr2)
        {

            SotaWelcome();


            Thread sendThread = new Thread(new ThreadStart(SendMessageToSocket));
            sendThread.Start(); //старт потока

            ////функция ожидает ответа от соты и повторяется рекурсией пока тип сообщения не будет 21
            Thread receiveThread = new Thread(new ThreadStart(ReceiveMessageFromSocket));
            receiveThread.Start(); //старт потока

            //SendMessageFromSocket(sender);
        }


        public void SotaWelcome()
        {
            #region welcome пакет
            char[] Text = new char[256];        //формируем welcome пакет
            char[] Text2 = new char[256];       //формируем welcome пакет
            string name = "Иванов Иван";        //формируем welcome пакет
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
            pac.fromind = 2001;
            #endregion

            byte[] arr = sota.StructToBytes(pac);           //переводим стуктуру шапки в последовательность байт
            byte[] arr2 = sota.StructToBytes2(welcome);     //переводим welcome

            // Устанавливаем удаленную точку для сокета
            IPAddress ipAddr = IPAddress.Parse(ip);//ipHost.AddressList[0]; 
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, port);

            CheckConnection();

            sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            this.RunOnUiThread(() => { Toast.MakeText(this.ApplicationContext, "Sender created!", ToastLength.Short).Show(); });

            // Соединяем сокет с удаленной точкой
            sender.Connect(ipEndPoint);

            this.RunOnUiThread(() => { Toast.MakeText(this.ApplicationContext, "Sender connected!", ToastLength.Short).Show(); });
            // Отправляем данные через сокет
            int bytesSent = sender.Send(arr);
            int bytesSent2 = sender.Send(arr2);

            this.RunOnUiThread(() => { Toast.MakeText(this.ApplicationContext, "Sender sended data!", ToastLength.Short).Show(); });
        }

        public void SendMessageToSocket()
        {
            DataSota.TSOTAKOSMOPARKMESAGEANDROID tsota = new DataSota.TSOTAKOSMOPARKMESAGEANDROID();

            DataSota.TsotaPaket pac = new DataSota.TsotaPaket();
            pac.uni = long.Parse("8642106494615744221");
            pac.typemes = 5101;
            pac.pack = 0;
            pac.size = Marshal.SizeOf(tsota);
            pac.packsize = Marshal.SizeOf(tsota);
            pac.fromind = 2001;
            tsota.Index = 702;
            tsota.Type = 3;

            byte[] arr = sota.StructToBytes(pac);
            byte[] arr2 = sota.StructToBytes4(tsota);

            // Устанавливаем удаленную точку для сокета
            IPAddress ipAddr = IPAddress.Parse(ip);
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, port);

            sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // Соединяем сокет с удаленной точкой
            sender.Connect(ipEndPoint);

            // Отправляем данные через сокет
            int bytesSent = sender.Send(arr);
            int bytesSent2 = sender.Send(arr2);
        }

        //функция ожидает ответа от соты и повторяется рекурсией пока тип сообщения не будет 21 
        public void ReceiveMessageFromSocket()
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

                            inputdata = "Connect" + paket.fromind;
                        }

                    }
                    else   //пакет нужно дособирать
                    {

                    }

                    // Используем рекурсию для неоднократного вызова SendMessageFromSocket()  
                    if (paket.typemes != 21)
                        ReceiveMessageFromSocket();
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

