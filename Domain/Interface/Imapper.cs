namespace Domain.Interface;

/// <summary>
/// Generic mapper interface for converting between entities and DTOs
/// </summary>
/// <typeparam name="TEntity">Domain entity type</typeparam>
/// <typeparam name="TDto">Data transfer object type</typeparam>
public interface IMapper<TEntity, TDto>
    where TEntity : class
    where TDto : class
{
    /// <summary>
    /// Maps an entity to a DTO
    /// </summary>
    TDto MapToDto(TEntity entity);

    /// <summary>
    /// Maps a DTO to an entity
    /// </summary>
    TEntity MapToEntity(TDto dto);

    /// <summary>
    /// Maps a collection of entities to DTOs
    /// </summary>
    IEnumerable<TDto> MapToDtoList(IEnumerable<TEntity> entities);

    /// <summary>
    /// Maps a collection of DTOs to entities
    /// </summary>
    IEnumerable<TEntity> MapToEntityList(IEnumerable<TDto> dtos);

    /// <summary>
    /// Updates an existing entity with values from a DTO
    /// </summary>
    void UpdateEntity(TEntity entity, TDto dto);
}