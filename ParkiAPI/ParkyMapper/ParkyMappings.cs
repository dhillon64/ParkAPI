using AutoMapper;
using ParkiAPI.Models;
using ParkiAPI.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkyAPI.ParkyMapper
{
    public class ParkyMappings : Profile

    {
        public ParkyMappings()
        {
            CreateMap<NationalPark, NationalParkDto>().ReverseMap();

        }

    }
}
