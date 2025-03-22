using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsersMgr.Domain.ValueObjects
{
    //record一般用于定于数据类型，大部分情况下，record定义的数据类型在赋值之后，是不能修改的
    public record PhoneNumber(int RegionNumber, string Number);
}
