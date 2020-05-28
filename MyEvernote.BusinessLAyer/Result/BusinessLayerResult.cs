using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyEvernote.Entities.Messages;

namespace MyEvernote.BusinessLAyer.Result
{
    public class BusinessLayerResult<T> where T : class
    {
        public List<ErrorMesageObj> Erros { get; set; }
        public T Result { get; set; }

        public BusinessLayerResult()
        {
            Erros = new List<ErrorMesageObj>();
        }

        public void AddError(ErrorMessage code, string message)
        {
            Erros.Add(new ErrorMesageObj()
            {
                Code = code,
                Message = message
            });
        }
    }
}
