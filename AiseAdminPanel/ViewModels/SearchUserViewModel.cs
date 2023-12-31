﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BasketWebPanel.ViewModels
{
    public class SearchUserViewModel : BaseViewModel
    {
        public SearchUserViewModel()
        {
            Users = new List<UserBindingModel>();
        }

        public List<UserBindingModel> Users { get; set; }

        public SelectList StatusOptions { get; internal set; }
    }

    public class UserBindingModel : BaseViewModel
    {
        public UserBindingModel()
        {
            UserAddresses = new List<UserAddressBindingModel>();
            PaymentCards = new List<UserPaymentCardBindingModel>();
        }

        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FullName { get; set; }

        public string ProfilePictureUrl { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string AccountType { get; set; }

        public string ZipCode { get; set; }

        public string DateofBirth { get; set; }

        public short? SignInType { get; set; }

        public string UserName { get; set; }

        public short? Status { get; set; }

        public bool EmailConfirmed { get; set; }

        public bool PhoneConfirmed { get; set; }

        public bool IsDeleted { get; set; }

        public string StatusName { get; set; }

        public bool IsChecked { get; set; }

        public List<UserAddressBindingModel> UserAddresses;

        public List<UserPaymentCardBindingModel> PaymentCards { get; set; }

    }

    public class UserPaymentCardBindingModel
    {
        public int Id { get; set; }

        public string CardNumber { get; set; }

        public DateTime ExpiryDate { get; set; }

        public string CCV { get; set; }

        public string NameOnCard { get; set; }

        public int CardType { get; set; } //1 for Credit, 2 for Debit

        public int User_ID { get; set; }

        public bool IsDeleted { get; set; }
    }

    public class UserAddressBindingModel
    {
        public int Id { get; set; }

        public int User_ID { get; set; }

        public string Country { get; set; }

        public string City { get; set; }

        public string StreetName { get; set; }

        public string Floor { get; set; }

        public string Apartment { get; set; }

        public string NearestLandmark { get; set; }

        public string BuildingName { get; set; }

        public short Type { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsPrimary { get; set; }
    }

}