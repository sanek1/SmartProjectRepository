using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;
using TestSmartProject.Model;
using TestSmartProject;
using System.Threading;
using System.Runtime.InteropServices;
using System.Net.Sockets;

namespace TestSmartProject.ViewModel
{
    public class ListViewMainPage : INotifyPropertyChanged
    {
        Socket sender;
        private ulong _idUni;
        public ObservableCollection<ViewModelMainPage> Users { get; set; }
        public string message { get; set; }
 
        public event PropertyChangedEventHandler PropertyChanged;
 
        public ICommand CreateUserCommand { protected set; get; }
        public ICommand DeleteUserCommand { protected set; get; }
        public ICommand SaveUserCommand { protected set; get; }
        public ICommand BackCommand { protected set; get; }
        public ICommand ConnectSotaCommand { protected set; get; }
        ViewModelMainPage selectedUser;
 
        public INavigation Navigation { get; set;}

        public ListViewMainPage()
        {
            Users = new ObservableCollection<ViewModelMainPage>();
            CreateUserCommand = new Command(CreateUser);
            DeleteUserCommand = new Command(DeleteUser);
            SaveUserCommand = new Command(SaveUser);
            BackCommand = new Command(Back);
            ConnectSotaCommand = new Command(ConnectSota);
        }

        public ViewModelMainPage SelectedUser
        {
            get { return selectedUser; }
            set
            {
                if (selectedUser != value)
                {
                    ViewModelMainPage tempUser = value;
                    selectedUser = null;
                    OnPropertyChanged("SelectedUser");
                    Navigation.PushAsync(new UserMainPage(tempUser));
                }
            }
        }
        protected void OnPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
 
        private void CreateUser()
        {
            Navigation.PushAsync(new UserMainPage(new ViewModelMainPage() { ListViewModel = this }));
        }
        private void Back()
        {
            Navigation.PopAsync();
        }
        private void SaveUser(object userObject)
        {
            ViewModelMainPage user = userObject as ViewModelMainPage;
            if (user != null && user.IsValid)
            {
                Users.Add(user);
            }
            //Back();
            Navigation.PushAsync(new ViewKosmopark(this));
        }
        private void DeleteUser(object userObject)
        {
            ViewModelMainPage user = userObject as ViewModelMainPage;
            if (user != null)
            {
                Users.Remove(user);
            }
            Back();
        }
        private void ConnectSota()
        {
            _idUni = 88;
            ConnectSota sonnect = new ConnectSota(2001, "192.168.12.33", "TEST", ref _idUni);
            sender = sonnect.SendWelcome();
            //функция ожидает ответа от соты и повторяется рекурсией пока тип сообщения не будет 21
            Thread receiveThread = new Thread(new ThreadStart(SendMessageFromSocket));
            receiveThread.Start(); //старт потока
        }



        public string Message
        {
            get { return message; }
            set
            {
                message = value;
                OnPropertyChanged("Message");
            }
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
                            int count = 0;
                            for (int i = 0; i < mess.Length; i++)
                            {
                                if (mess[i] == 0)
                                {
                                    break;
                                }
                                count++;
                            }


                            //char aa= BitConverter.ToChar(byteMess,32);
                            Encoding dec = Encoding.GetEncoding(1251);

                            char[] bb = new char[count-1];
                            int q = dec.GetChars(mess, 0, count-1, bb, 0);
                            string asciiString = new string(bb);
                            Message = asciiString;

                            //uia

                            //Button btn = FindViewById<Button>(Resource.Id.button1);
                            //btn.Text = "43333";

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
            //Finish();
            Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
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
