using QPP.Collections;
using QPP.Command;
using QPP.ComponentModel;
using QPP.Messaging;
using QPP.Navigation;
using QPP.Runtime;
using QPP.Threading;
using QPP.Wpf;
using QPP.Wpf.Command;
using QPP.Wpf.Threading;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace QPP.Wpf.Controls.Progress
{
    [QPP.Modularity.Presenter(Uri="/QPP.Wpf;component/Controls/Progress/ProgressBox.xaml")]
    public class Progresser : ViewModelBase
    {
        public static Progresser Default = new Progresser();

        public ObservableCollection<ProgressItem> Items { get; set; }

        protected override void Initialize()
        {
            base.Initialize();
            Items = new ObservableCollection<ProgressItem>();
        }

        public void Register()
        {
            RuntimeContext.Service.Messenger.Register<GenericMessage<ProgressItem>>(this, (e) =>
            {
                AddItem(e.Content);
                RuntimeContainers.Current.Open("/QPP.Master.Wpf;component/Progress/ProgressBox.xaml");
            });
        }

        [RelayCommand]
        public ICommand CancelCommand
        {
            get
            {
                return CommandContext.GetCommand<ProgressItem>(() => CancelCommand, (job) => 
                {
                    if (job.Worker.IsBusy)
                    {
                        job.Worker.CancelAsync();
                        RuntimeContext.Service.Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ")
                            + RuntimeContext.Service.L10N.GetText("標題.進度") + ":" + job.Name + RuntimeContext.Service.L10N.GetText("進度.取消"));
                    }
                    else
                        Items.Remove(job);
                });
            }
        }

        public void AddItem(ProgressItem item)
        {
            RuntimeContext.Service.Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ")
                + RuntimeContext.Service.L10N.GetText("標題.進度") + ":" + item.Name + RuntimeContext.Service.L10N.GetText("進度.開始"));
            item.Worker.RunWorkerCompleted += (s, e) =>
            {
                if (e.Error == null)
                {
                    Items.Remove(item);
                    RuntimeContext.Service.Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ")
                        + RuntimeContext.Service.L10N.GetText("標題.進度") + ":" + item.Name + RuntimeContext.Service.L10N.GetText("進度.結束"));
                }
                else
                {
                    item.HasError = true;
                    item.Message = RuntimeContext.Service.L10N.GetText("發生錯誤:") + e.Error.Message;
                    item.Details = e.Error.ToString();
                    RuntimeContext.Service.Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") 
                        + RuntimeContext.Service.L10N.GetText("標題.進度") + ":" + item.Name + Environment.NewLine
                        + RuntimeContext.Service.L10N.GetText("發生錯誤:") + item.Details);
                }
            };
            Items.Add(item);
        }
    }
}
