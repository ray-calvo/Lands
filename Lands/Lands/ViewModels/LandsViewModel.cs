﻿namespace Lands.ViewModels
{
    using GalaSoft.MvvmLight.Command;
    using Models;
    using Services;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows.Input;
    using Xamarin.Forms;

    public class LandsViewModel : BaseViewModel
    {
        #region Services
        private ApiService apiservice;
        #endregion
        #region Attributes
        private ObservableCollection<LandItemViewModel> lands;
        private bool isRefreshing;
        private string filter;
        private List<Land> landsList; 
        #endregion
        #region Properties
        public ObservableCollection<LandItemViewModel> Lands
        {
            get { return this.lands; }
            set { SetValue(ref this.lands, value); }
        }
        public bool IsRefreshing 
        {
            get { return this.isRefreshing; }
            set { SetValue(ref this.isRefreshing, value); }
        }
        public string Filter
        {
            get { return this.filter; }
            set
            {
                SetValue(ref this.filter, value);
                this.Search();
            }
        }
        #endregion
        #region Constructors
        public LandsViewModel()
        {
            this.apiservice = new ApiService();
            this.LoadLands();
        }
        #endregion
        #region Methods
        private async void LoadLands()
        {
            this.IsRefreshing = true;
            var connection = await this.apiservice.CheckConnection();
            if (!connection.IsSuccess)
            {
                this.IsRefreshing = false;
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    connection.Message,
                    "Accept");
                await Application.Current.MainPage.Navigation.PopAsync();
                return;

            }
            {

            }
            var response = await this.apiservice.GetList<Land>(
                "http://restcountries.eu",
                "/rest",
                "/v2/all");
            if (!response.IsSuccess)
            {
                this.IsRefreshing = false;
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    response.Message,
                    "Accept");
                await Application.Current.MainPage.Navigation.PopAsync();
                return;
            }
            this.landsList = (List<Land>)response.Result;
            this.Lands = new ObservableCollection<LandItemViewModel>(
                this.ToLandItemViewModel ());
            this.IsRefreshing = false;
        }

        #region Methods
        private IEnumerable<LandItemViewModel> ToLandItemViewModel()
        {
            return this.landsList.Select(l => new LandItemViewModel
            {
                Alpha2Code = l.Alpha2Code,
                Alpha3Code = l.Alpha3Code,
                AltSpellings = l.AltSpellings,
                Area = l.Area,
                Borders = l.Borders,
                CallingCodes = l.CallingCodes,
                Capital = l.Capital,
                Cioc = l.Cioc,
                Currencies = l.Currencies,
                Demonym = l.Demonym,
                Flag = l.Flag,
                Gini = l.Gini,
                Languages = l.Languages,
                Latlng = l.Latlng,
                Name = l.Name,
                NativeName = l.NativeName,
                NumericCode = l.NumericCode,
                Population = l.Population,
                Region = l.Region,
                RegionalBlocs = l.RegionalBlocs,
                Subregion = l.Subregion,
                Timezones = l.Timezones,
                TopLevelDomain = l.TopLevelDomain,
                Translations = l.Translations,
            }); 
        } 
        #endregion
        #endregion
        #region Commands
        public ICommand RefreshCommand
        {
            get
            {
                return new RelayCommand(LoadLands);
            }
            
        }
        public ICommand SearchCommand
        {
            get
            {
                return new RelayCommand(Search);
            }

        }

        private void Search()
        {
            if (string.IsNullOrEmpty(this.Filter))
            {
                this.Lands = new ObservableCollection<LandItemViewModel>(
                 this.ToLandItemViewModel());
            }
            else
            {
                this.Lands = new ObservableCollection<LandItemViewModel>(
                    this.ToLandItemViewModel().Where(
                        l => l.Name.ToLower().Contains(this.filter.ToLower()) ||
                             l.Capital.ToLower().Contains(this.filter.ToLower())));
                
            }
        }

        
        #endregion

    }
}
