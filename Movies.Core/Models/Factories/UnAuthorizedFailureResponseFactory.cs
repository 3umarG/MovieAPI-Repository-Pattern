using Movies.Core.Interfaces;
using Movies.Core.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Core.Models.Factories
{
    public class UnAuthorizedFailureResponseFactory : IResponseFactory
    {
        private readonly UnAuthorizedFailureResponse _response;

        public UnAuthorizedFailureResponseFactory()
        {
            _response = new UnAuthorizedFailureResponse();
        }
        public IResponse CreateResponse()
        {
            return _response;
        }
    }
}
