[Route("api/[controller]")]
public class ServiceTypesController : Controller
{
...

    [HttpPut("{typeId}/spec")] |\label{line:httpPut}|
    public virtual Task<ServiceAttributeSpecification> CreateServiceAttributeSpecification(int typeId, [FromBody] ServiceAttributeSpecification spec)
    {
        HttpContext.Response.StatusCode = (int) HttpStatusCode.Created;
        spec.ServiceTypeId = typeId;
        return _attributeSpecificationService.CreateAttributeSpecificationAsync(spec);
    }
...
