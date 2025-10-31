using AutoMapper;
using MediatR;
using SMS.Application.Commands.Academic;
using SMS.Application.Interfaces;
using SMS.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Application.Handlers.Academic
{
    public class UpsertExamPaperHandler : IRequestHandler<UpsertExamPaperCommand, int>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        public UpsertExamPaperHandler(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }
        public Task<int> Handle(UpsertExamPaperCommand request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<ExamPaper>(request.Paper);
            return _uow.AcademicRepository.UpsertExamPaperAsync(cancellationToken, entity);
            
        }
    }
}
