using Airslip.Common.Repository.Interfaces;
using AutoMapper;

namespace Airslip.Identity.AutoMapper.Implementations
{
    public class AutoMapperModelMapper<TModel> : IModelMapper<TModel>
    {
        private readonly IMapper _mapper;

        public AutoMapperModelMapper(IMapper mapper)
        {
            _mapper = mapper;
        }

        public TEntity CreateEntity<TEntity>(TModel source)
        {
            return _mapper.Map<TModel, TEntity>(source);
        }

        public TEntity UpdateEntity<TEntity>(TModel source, TEntity destination)
        {
            return _mapper.Map<TModel, TEntity>(source, destination);
        }

        public TModel CreateModel<TEntity>(TEntity source)
        {
            return _mapper.Map<TEntity, TModel>(source);
        }
    }
}