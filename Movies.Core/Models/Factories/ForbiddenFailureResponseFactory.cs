using Movies.Core.Interfaces;
using Movies.Core.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Core.Models.Factories
{
    public class ForbiddenFailureResponseFactory : IResponseFactory
    {
        private readonly ForbiddenFailureResponse _response;

        public ForbiddenFailureResponseFactory()
        {
            _response = new ForbiddenFailureResponse();
        }

        public IResponse CreateResponse()
        {
            return _response;
        }
    }
}
