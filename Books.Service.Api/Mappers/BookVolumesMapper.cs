using System;
using AutoMapper;
using Books.Service.Internal.Api.Models;
using Google.Apis.Books.v1.Data;

namespace Books.Service.Internal.Api.Mappers
{
	public class BookVolumesMapper: Profile
	{
		public BookVolumesMapper()
		{
            CreateMap<Volumes, BookResultsModel>();

            CreateMap<Volume, BookInfoModel>();
            CreateMap<Volume.VolumeInfoData, VolumeInfoModel>();
            CreateMap<Volume.VolumeInfoData.IndustryIdentifiersData, IndustryIdentifier>();
            CreateMap<Volume.VolumeInfoData.ImageLinksData, ImageLink>();
        }
	}
}

