using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsersMgr.Domain.ValueObjects;

namespace UsersMgr.Domain
{
    //INotification是通知类型接口
    public record class UserAccessResultEvent(PhoneNumber PhoneNumber, UserAccessResult UserAccessResult): INotification;
}
