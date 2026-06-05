using Domain.DTOs;
using Domain.Mappers;
using Domain.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Repositories.Entities;
using Repositories.Enums;
using Repositories.Repositories.Interfaces;
using Repositories.Repositories.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Services.Services
{
    public class ShipmentDecisionService : IShipmentDecisionService
    {
        private readonly IShipmentDecisionRepository shipmentDecisionRepository;
        private readonly IBatchRepository batchRepository;
        private readonly IUserRepository userRepository;
        private readonly IQualityCertificateRepository qualityCertificateRepository;
        private readonly IProductRepository productRepository;

        public ShipmentDecisionService(
            IShipmentDecisionRepository shipmentDecisionRepository,
            IBatchRepository batchRepository,
            IUserRepository userRepository,
            IQualityCertificateRepository qualityCertificateRepository,
            IProductRepository productRepository)
        {
            this.shipmentDecisionRepository = shipmentDecisionRepository;
            this.batchRepository = batchRepository;
            this.userRepository = userRepository;
            this.qualityCertificateRepository = qualityCertificateRepository;
            this.productRepository = productRepository;
        }

        public async Task<List<ShipmentDecisionDTO>> GetDecisionsAsync(
            CancellationToken cancellationToken = default)
        {
            var decisions = await shipmentDecisionRepository
                .GetAllWithDetailsAsync(cancellationToken);

            return decisions
                .Select(ToDto)
                .ToList();
        }

        public async Task<List<BatchDTO>> GetBatchesAllowedForShipmentAsync(
            CancellationToken cancellationToken = default)
        {
            var batches = await batchRepository
                .GetBatchesAllowedForShipmentAsync(cancellationToken);

            var list = new List<BatchDTO>();

            foreach (var batch in batches)
            {
                var dto = EntityToDTOMapper.ToBatchDTO(batch);
                dto.Product = EntityToDTOMapper.ToProductDTO(productRepository.GetByIdAsync(dto.ProductId).Result);
                dto.CreatedByUser = EntityToDTOMapper.ToUserDTO(userRepository.GetByIdAsync(dto.CreatedByUserId).Result);

                list.Add(dto);
            }

            return list;
        }

        public async Task<ShipmentDecisionDTO?> GetDecisionByBatchIdAsync(
            int batchId,
            CancellationToken cancellationToken = default)
        {

            var decision = await shipmentDecisionRepository
                .GetByBatchIdNoTrackingAsync(batchId, cancellationToken);

            return decision is null ? null : ToDto(decision);
        }

        public async Task<ShipmentDecisionDTO> AllowShipmentAsync(
            int batchId,
            int userId,
            CancellationToken cancellationToken = default)
        {
            return await CreateOrUpdateDecisionAsync(
                batchId,
                userId,
                ShipmentDecisionStatus.Allowed,
                "Відвантаження дозволено на підставі сформованого сертифіката якості.",
                cancellationToken);
        }

        public async Task<ShipmentDecisionDTO> DenyShipmentAsync(
            int batchId,
            int userId,
            CancellationToken cancellationToken = default)
        {
            return await CreateOrUpdateDecisionAsync(
                batchId,
                userId,
                ShipmentDecisionStatus.Prohibited,
                "Відвантаження заборонено працівником складу.",
                cancellationToken);
        }

        private async Task<ShipmentDecisionDTO> CreateOrUpdateDecisionAsync(
            int batchId,
            int userId,
            ShipmentDecisionStatus status,
            string decisionText,
            CancellationToken cancellationToken)
        {

            var batch = await batchRepository.GetByIdAsync(batchId, cancellationToken);


            var user = await userRepository.GetByIdAsync(userId, cancellationToken);


            var certificate = await qualityCertificateRepository
                .GetByBatchIdAsync(batchId, cancellationToken);

            if (certificate is null)
                throw new InvalidOperationException(
                    "Неможливо прийняти рішення щодо відвантаження без сформованого сертифіката якості.");

            var decision = await shipmentDecisionRepository
                .GetByBatchIdForUpdateAsync(batchId, cancellationToken);

            if (decision is null)
            {
                decision = new ShipmentDecision
                {
                    BatchId = batchId,
                    CreatedAt = DateTime.UtcNow,
                    CreatedByUserId = userId
                };

                await shipmentDecisionRepository.AddAsync(decision, cancellationToken);
            }

            decision.Status = status;
            decision.DecisionText = decisionText;
            decision.CreatedAt = DateTime.UtcNow;
            decision.CreatedByUserId = userId;

            if (status == ShipmentDecisionStatus.Allowed)
            {
                batch.Status = BatchStatus.ReadyForShipment;
                batchRepository.Update(batch);
            }

            if (status == ShipmentDecisionStatus.Prohibited)
            {
                batch.Status = BatchStatus.Approved;
                batchRepository.Update(batch);
            }

            await shipmentDecisionRepository.SaveChangesAsync(cancellationToken);

            var updatedDecision = await shipmentDecisionRepository
                .GetByBatchIdNoTrackingAsync(batchId, cancellationToken);

            if (updatedDecision is null)
                throw new InvalidOperationException("Рішення було створено, але не знайдено після збереження.");

            return ToDto(updatedDecision);
        }

        private ShipmentDecisionDTO ToDto(ShipmentDecision decision)
        {
            return new ShipmentDecisionDTO
            {
                Id = decision.Id,
                BatchId = decision.BatchId,
                Batch = decision.Batch is null
                    ? null
                    : EntityToDTOMapper.ToBatchDTO(decision.Batch),
                Status = decision.Status,
                DecisionText = decision.DecisionText,
                CreatedAt = decision.CreatedAt,
                CreatedByUserId = decision.CreatedByUserId,
                CreatedByUser = decision.CreatedByUser is null
                    ? null
                    : EntityToDTOMapper.ToUserDTO(decision.CreatedByUser)
            };
        }
    }
}
