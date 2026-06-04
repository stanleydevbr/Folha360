#!/bin/bash
# RabbitMQ initialization script
# This script creates exchanges, queues and bindings for the Folha360 system

# Wait for RabbitMQ to be ready
sleep 10

# Create exchanges
rabbitmqadmin declare exchange name=domain-events type=topic durable=true
rabbitmqadmin declare exchange name=esocial type=direct durable=true

# Create main queues
rabbitmqadmin declare queue name=folha-calculada durable=true
rabbitmqadmin declare queue name=esocial-envio durable=true

# Create dead-letter queues
rabbitmqadmin declare queue name=folha-calculada-dlq durable=true
rabbitmqadmin declare queue name=esocial-envio-dlq durable=true

# Bindings for domain-events exchange
rabbitmqadmin declare binding source=domain-events destination=folha-calculada routing_key="folha.calculada.*"
rabbitmqadmin declare binding source=domain-events destination=folha-calculada-dlq routing_key="folha.calculada.dlq"

# Bindings for esocial exchange
rabbitmqadmin declare binding source=esocial destination=esocial-envio routing_key="esocial.envio"
rabbitmqadmin declare binding source=esocial destination=esocial-envio-dlq routing_key="esocial.envio.dlq"

echo "RabbitMQ topology initialized successfully"
