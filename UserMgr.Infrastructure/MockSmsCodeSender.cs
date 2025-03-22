using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsersMgr.Domain;
using UsersMgr.Domain.ValueObjects;

namespace UserMgr.Infrastructure
{
    public class MockSmsCodeSender : ISmsCodeSender
    {
        public Task SendAsync(PhoneNumber phoneNumber, string code)
        {
            Console.WriteLine($"向{phoneNumber.RegionNumber}_{phoneNumber.Number}发送验证码{code}");
            return Task.CompletedTask;
        }
    }
}
