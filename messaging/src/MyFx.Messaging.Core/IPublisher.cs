using System;
using System.Threading.Tasks;

namespace MyFx.Messaging.Core
{
    ///WIP
    public interface IPublisher
    {

        Task SendCommandAsync(CommandBase command);
    }
}