using PC.Services.Core.EmailModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PC.Services.Core.Interfaces
{
    public interface ISendEmail
    {
        public Task<bool> SendEmail(EmailInfo emailInfo);
    }
}
