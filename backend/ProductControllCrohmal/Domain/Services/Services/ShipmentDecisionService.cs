using Domain.DTOs;
using Domain.Mappers;
using Domain.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Repositories.Entities;
using Repositories.Enums;
using Repositories.Repositories.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Services.Services
{
    public class ShipmentDecisionService : IShipmentDecisionService
    {
        private readonly ShipmentDecisionRepository shipmentDecisionRepository;
        private readonly BatchRepository batchRepository;
        private readonly UserRepository userRepository;

        public ShipmentDecisionService(
            ShipmentDecisionRepository shipmentDecisionRepository,
            BatchRepository batchRepository,
            UserRepository userRepository)
        {
            this.shipmentDecisionRepository = shipmentDecisionRepository;
            this.batchRepository = batchRepository;
            this.userRepository = userRepository;
        }

        public async Task<ShipmentDecisionDTO> CreateDecisionAsync(
            int batchId,
            int userId,
            CancellationToken cancellationToken = default)
        {
            if (batchId <= 0)
                throw new ArgumentException("Некорректный ID партии.");

            if (userId <= 0)
                throw new ArgumentException("Некорректный ID пользователя.");

            var existingDecision = await shipmentDecisionRepository
                .GetByBatchIdAsync(batchId, cancellationToken);

            if (existingDecision is not null)
                return EntityToDTOMapper.ToShipmentDecisionDTO(existingDecision);

            var batch = await batchRepository.GetWithDetailsAsync(batchId, cancellationToken);

            if (batch is null)
                throw new InvalidOperationException("Партия не найдена.");

            var user = await userRepository.GetByIdAsync(userId, cancellationToken);

            if (user is null)
                throw new InvalidOperationException("Пользователь не найден.");

            var isAllowedForShipment =
                batch.Status == BatchStatus.Approved &&
                batch.QualityCertificate is not null;

            var decision = new ShipmentDecision
            {
                BatchId = batchId,
                Status = isAllowedForShipment
                    ? ShipmentDecisionStatus.Allowed
                    : ShipmentDecisionStatus.Prohibited,
                DecisionText = isAllowedForShipment
                    ? "Партію дозволено до відвантаження"
                    : "Партію заборонено до відвантаження",
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = userId
            };

            await shipmentDecisionRepository.AddAsync(decision, cancellationToken);
            await shipmentDecisionRepository.SaveChangesAsync(cancellationToken);

            decision.Batch = batch;
            decision.CreatedByUser = user;

            return EntityToDTOMapper.ToShipmentDecisionDTO(decision);
        }

        public async Task<List<BatchDTO>> GetBatchesAllowedForShipmentAsync(
            CancellationToken cancellationToken = default)
        {
            var batches = await batchRepository
                .QueryNoTracking()
                .Include(x => x.Product)
                .Include(x => x.CreatedByUser)
                .Include(x => x.QualityCertificate)
                .Where(x =>
                    x.Status == BatchStatus.Approved &&
                    x.QualityCertificate != null)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync(cancellationToken);

            return batches
                .Select(EntityToDTOMapper.ToBatchDTO)
                .ToList();
        }

        public async Task<ShipmentDecisionDTO?> GetDecisionByBatchIdAsync(
            int batchId,
            CancellationToken cancellationToken = default)
        {
            if (batchId <= 0)
                throw new ArgumentException("Некорректный ID партии.");

            var decision = await shipmentDecisionRepository
                .GetByBatchIdAsync(batchId, cancellationToken);

            if (decision is null)
                return null;

            return EntityToDTOMapper.ToShipmentDecisionDTO(decision);
        }
    }
}
