using Domain.DTOs;
using Domain.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Repositories.Entities;

namespace ProductControllCrohmal.Controllers
{
    [Route("api/shipment-decisions")]
    public class ShipmentDecisionController : BaseApiController
    {
        private readonly IShipmentDecisionService shipmentDecisionService;

        public ShipmentDecisionController(IShipmentDecisionService shipmentDecisionService)
        {
            this.shipmentDecisionService = shipmentDecisionService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ShipmentDecisionDTO>>> GetDecisions(
            CancellationToken cancellationToken)
        {
            try
            {
                var decisions = await shipmentDecisionService
                    .GetDecisionsAsync(cancellationToken);

                return Ok(decisions);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("allowed-batches")]
        public async Task<ActionResult<List<BatchDTO>>> GetBatchesAllowedForShipment(
             CancellationToken cancellationToken)
        {
            try
            {
                var batches = await shipmentDecisionService
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
                var decision = await shipmentDecisionService
                    .GetDecisionByBatchIdAsync(batchId, cancellationToken);

                if (decision is null)
                {
                    return NotFound(new
                    {
                        message = "Рішення щодо відвантаження для партії не знайдено."
                    });
                }

                return Ok(decision);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost("batch/{batchId:int}/allow")]
        public async Task<ActionResult<ShipmentDecisionDTO>> AllowShipment(
            int batchId,
            [FromQuery] int userId,
            CancellationToken cancellationToken)
        {
            try
            {
                var decision = await shipmentDecisionService
                    .AllowShipmentAsync(batchId, userId, cancellationToken);

                return Ok(decision);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost("batch/{batchId:int}/deny")]
        public async Task<ActionResult<ShipmentDecisionDTO>> DenyShipment(
            int batchId,
            [FromQuery] int userId,
            CancellationToken cancellationToken)
        {
            try
            {
                var decision = await shipmentDecisionService
                    .DenyShipmentAsync(batchId, userId, cancellationToken);

                return Ok(decision);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
