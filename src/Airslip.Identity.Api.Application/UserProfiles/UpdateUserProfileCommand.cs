using Airslip.Common.Contracts;
using MediatR;
using System;

namespace Airslip.BankTransactions.Api.Application.UserProfiles
{
    public class UpdateUserProfileCommand : IRequest<IResponse>
    {
        public string UserId { get; }
        public string? FirstName { get; }
        public string? Surname { get; }
        public string? Gender { get; }
        public DateTimeOffset? DateOfBirth { get; }
        public string? Postalcode { get; }
        public string? FirstLineAddress { get; }
        public string? SecondLineAddress { get; }
        public string? City { get; }
        public string? County { get; }
        public string? Country { get; }

        public UpdateUserProfileCommand(
            string userId,
            string? firstName,
            string? surname,
            string? gender,
            DateTimeOffset? dateOfBirth,
            string? postalcode,
            string? firstLineAddress,
            string? secondLineAddress,
            string? city,
            string? county,
            string? country)
        {
            UserId = userId;
            FirstName = firstName;
            Surname = surname;
            Gender = gender;
            DateOfBirth = dateOfBirth;
            Postalcode = postalcode;
            FirstLineAddress = firstLineAddress;
            SecondLineAddress = secondLineAddress;
            City = city;
            County = county;
            Country = country;
        }
    }
}