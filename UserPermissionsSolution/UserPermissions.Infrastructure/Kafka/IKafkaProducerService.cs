﻿namespace UserPermissions.Infrastructure.Kafka
{
    public interface IKafkaProducerService
    {
        Task SendMessageAsync(string topic, string message);
    }
}