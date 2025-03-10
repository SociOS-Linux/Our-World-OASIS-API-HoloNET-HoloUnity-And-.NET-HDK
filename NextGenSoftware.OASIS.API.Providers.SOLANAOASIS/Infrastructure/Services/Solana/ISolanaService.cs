﻿using System.Threading.Tasks;
using NextGenSoftware.OASIS.API.Core.Helpers;
using NextGenSoftware.OASIS.API.Providers.SOLANAOASIS.Entities.DTOs.Requests;
using NextGenSoftware.OASIS.API.Providers.SOLANAOASIS.Entities.DTOs.Responses;

namespace NextGenSoftware.OASIS.API.Providers.SOLANAOASIS.Infrastructure.Services.Solana
{
    public interface ISolanaService
    {
        Task<OASISResult<ExchangeTokenResult>> ExchangeTokens(ExchangeTokenRequest exchangeTokenRequest);
        Task<OASISResult<MintNftResult>> MintNft(MintNftRequest mintNftRequest);
        Task<OASISResult<SendTransactionResult>> SendTransaction(SendTransactionRequest sendTransactionRequest);
        Task<OASISResult<GetNftMetadataResult>> GetNftMetadata(GetNftMetadataRequest getNftMetadataRequest);
        Task<OASISResult<GetNftWalletResult>> GetNftWallet(GetNftWalletRequest getNftWalletRequest);
    }
}