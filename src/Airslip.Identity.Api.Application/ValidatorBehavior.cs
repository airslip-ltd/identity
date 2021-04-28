using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Airslip.Identity.Api.Application
{
    public class ValidatorBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>, IDisposable
        where TRequest : notnull
    {
        private readonly ILogger _logger;
        private readonly IServiceScope _scope;
        private readonly IList<IValidator> _validators;
        private readonly Assembly MediatRAssembly = typeof(IBaseRequest).Assembly;
        private readonly ConcurrentDictionary<Type, List<Type>> ValidationLookupTable = new();

        public ValidatorBehavior(IServiceProvider serviceProvider)
        {
            _logger = Log.Logger;

            _scope = serviceProvider.CreateScope();

            List<Type> validatorTypes = ValidationLookupTable.GetOrAdd(typeof(TRequest), value =>
            {
                return value
                    .GetInterfaces()
                    .Where(type => type.Assembly != MediatRAssembly)
                    .Select(type => typeof(IValidator<>).MakeGenericType(type))
                    .ToList();
            });

            _validators = new List<IValidator>();

            if (_scope.ServiceProvider.GetService(typeof(IValidator<TRequest>)) is IValidator commandValidator)
                _validators.Add(commandValidator);

            foreach (Type? type in validatorTypes)
            {
                if (_scope.ServiceProvider.GetService(type) is not IValidator instance)
                    throw new NullReferenceException($"Unable to resolve validator for interface {type.Name}");

                _validators.Add(instance);
            }
        }

        public void Dispose()
        {
            _scope.Dispose();
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            string type = typeof(TRequest).Name;
            Stopwatch stopwatch = Stopwatch.StartNew();

            List<ValidationFailure> failures = _validators
                .Select(validator => validator.Validate(new ValidationContext<object>(request)))
                .SelectMany(validationResult => validationResult.Errors)
                .Where(validationFailure => validationFailure != null)
                .ToList();

            if (failures.Any())
            {
                ValidationException exception = new($"Validation errors occured for type {type}", failures);
                _logger.Warning(exception, "Validation failed after {Duration}ms", stopwatch.ElapsedMilliseconds);
                throw exception;
            }

            _logger.Debug("Validation passed after {Duration}ms", stopwatch.ElapsedMilliseconds);

            stopwatch.Restart();

            TResponse response = await next();

            _logger.Information("Message {Type} handled by {Application} in {Duration}ms", type, "MediatR",
                stopwatch.ElapsedMilliseconds);
            return response;
        }
    }
}