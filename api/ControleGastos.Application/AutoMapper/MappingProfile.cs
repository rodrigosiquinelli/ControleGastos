using AutoMapper;
using ControleGastos.Application.DTOs.Categoria;
using ControleGastos.Application.DTOs.Pessoa;
using ControleGastos.Application.DTOs.Transacao;
using ControleGastos.Domain.DTOs;
using ControleGastos.Domain.Models;

namespace ControleGastos.Application.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Pessoa, PessoaDto>();

            CreateMap<Categoria, CategoriaDto>();

            CreateMap<Transacao, TransacaoDto>()
                .ForMember(dest => dest.CategoriaDescricao, opt => opt.MapFrom(src => src.Categoria.Descricao))
                .ForMember(dest => dest.PessoaNome, opt => opt.MapFrom(src => src.Pessoa.Nome));
        }
    }
}
