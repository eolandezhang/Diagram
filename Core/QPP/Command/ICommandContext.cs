using QPP.Modularity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace QPP.Command
{
    /// <summary>
    /// 命令服務
    /// </summary>
    public interface ICommandContext : INotifyPropertyChanged
    {
        /// <summary>
        /// 命令執行事件
        /// </summary>
        event EventHandler<CancelCommandEventArgs> PreExecute;
        /// <summary>
        /// 命令執行事件
        /// </summary>
        event EventHandler<CommandEventArgs> Executed;
        /// <summary>
        /// 命令集合
        /// </summary>
        CommandCollection Commands { get; }
        /// <summary>
        /// 註冊命令
        /// </summary>
        /// <exception cref="CommandIllegalException">CommandModel.Name已經註冊</exception>
        /// <param name="command"></param>
        void Register(ICommandModel command);
        /// <summary>
        /// 註銷命令
        /// </summary>
        /// <param name="command"></param>
        void Unregister(string command);
        /// <summary>
        /// 執行指定命令
        /// </summary>
        /// <exception cref="CommandIllegalException">commandName沒有註冊</exception>
        /// <param name="commandName">不能為空</param>
        /// <param name="param"></param>
        void Execute(string commandName, object param = null);
        /// <summary>
        /// 斷定指定命令是否能執行
        /// </summary>
        /// <param name="commandName">不能為空</param>
        /// <param name="param"></param>
        /// <returns></returns>
        bool CanExecute(string commandName, object param = null);
        ///// <summary>
        ///// 模塊
        ///// </summary>
        //IView Module { get; }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="viewModel"></param>
        void Init(IPresenter viewModel);
    }
}
