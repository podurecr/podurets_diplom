using Domain.DTOs;
using Domain.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Repositories.Entities;

namespace ProductControllCrohmal.Controllers
{
    [Route("api/shipment-decisions")]
    public class ShipmentDecisionController : BaseApiController
    {
        private readonly IShipmentDecisionService _shipmentDecisionService;

        public ShipmentDecisionController(IShipmentDecisionService shipmentDecisionService)
        {
            _shipmentDecisionService = shipmentDecisionService;
        }

        [HttpPost("batch/{batchId:int}")]
        public async Task<ActionResult<ShipmentDecisionDTO>> CreateDecision(
            int batchId,
            [FromQuery] int userId,
            CancellationToken cancellationToken)
        {
            try
            {
                var decision = await _shipmentDecisionService
                    .CreateDecisionAsync(batchId, userId, cancellationToken);

                return Ok(decision);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("allowed-batches")]
        public async Task<ActionResult<List<ShipmentDecisionDTO>>> GetBatchesAllowedForShipment(
            CancellationToken cancellationToken)
        {
            try
            {
                var batches = await _shipmentDecisionService
                    .GetBatchesAllowedForShipmentAsync(cancellationToken);

                return Ok(batches);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("batch/{batchId:int}")]
        public async Task<ActionResult<ShipmentDecisionDTO>> GetDecisionByBatchId(
            int batchId,
            CancellationToken cancellationToken)
        {
            try
            {
                var decision = await _shipmentDecisionService
                    .GetDecisionByBatchIdAsync(batchId, cancellationToken);

                if (decision is null)
                    return NotFound(new { message = "Решение по отгрузке для партии не найдено." });

                return Ok(decision);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
