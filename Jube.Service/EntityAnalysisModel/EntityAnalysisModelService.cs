using AutoMapper;
using Jube.Data.Context;
using Jube.Data.Repository;
using Jube.Service.Dto.EntityAnalysisModel;

namespace Jube.Service.EntityAnalysisModel;

public class EntityAnalysisModelService
{
    private readonly Mapper mapper;
    private readonly EntityAnalysisModelRepository repository;

    public EntityAnalysisModelService(DbContext dbContext, string userName)
    {
        repository = new EntityAnalysisModelRepository(dbContext, userName);

        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Data.Poco.EntityAnalysisModel, EntityAnalysisModelDto>();
            cfg.CreateMap<EntityAnalysisModelDto, Data.Poco.EntityAnalysisModel>();
            cfg.CreateMap<List<Data.Poco.EntityAnalysisModel>, List<EntityAnalysisModelDto>>()
                .ForMember("Item", opt => opt.Ignore());
        });

        mapper = new Mapper(config);
    }

    public IEnumerable<EntityAnalysisModelDto> Get()
    {
        return mapper.Map<List<EntityAnalysisModelDto>>(repository.Get());
    }

    public EntityAnalysisModelDto Get(int id)
    {
        return mapper.Map<EntityAnalysisModelDto>(
            repository.GetById(id));
    }

    public EntityAnalysisModelDto Insert(EntityAnalysisModelDto model)
    {
        return mapper.Map<EntityAnalysisModelDto>(repository.Insert(
            repository.Insert(
                mapper.Map<Data.Poco.EntityAnalysisModel>(mapper.Map<Data.Poco.EntityAnalysisModel>(model)))));
    }

    public EntityAnalysisModelDto Update(EntityAnalysisModelDto model)
    {
        return mapper.Map<EntityAnalysisModelDto>(
            repository.Update(
                mapper.Map<Data.Poco.EntityAnalysisModel>(mapper.Map<Data.Poco.EntityAnalysisModel>(model))));
    }

    public void Delete(int id)
    {
        repository.Delete(id);
    }
}