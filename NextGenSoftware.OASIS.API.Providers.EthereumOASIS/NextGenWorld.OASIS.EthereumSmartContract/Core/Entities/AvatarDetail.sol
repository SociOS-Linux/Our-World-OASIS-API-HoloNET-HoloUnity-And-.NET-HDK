// SPDX-License-Identifier: UNLICENSED
// compiler version must be greater than or equal to 0.8.3 and less than 0.9.0
pragma solidity ^0.8.9;

import "../Enums/AvatarType.sol";
import "../Enums/ProviderType.sol";
import "../Common/Achievement.sol";
import "../Common/AvatarAttributes.sol";
import "../Common/AvatarAura.sol";
import "../Enums/OASISType.sol";
import "../Enums/DimensionLevel.sol";
import "../Enums/Color.sol";
import "../Common/GeneKey.sol";
import "../Common/AvatarGift.sol";
import "../Common/HeartRateEntry.sol";
import "../Common/HumanDesign.sol";
import "../Common/InventoryItem.sol";
import "../Common/KarmaAkashicRecord.sol";
import "../Common/Spell.sol";
import "../Common/AvatarSkills.sol";
import "../Common/AvatarStats.sol";
import "../Common/Omiverse.sol";
import "../Common/AvatarChakras.sol";

struct AvatarDetail {
    uint256 EntityId;
    string AvatarId;
    string Info;
    // string Title;
    // string FirstName;
    // string LastName;
    // string FullName;
    // string Username;
    // string Email;
    // Achievement[] Achievements;
    // string Address;
    // AvatarAttributes Attributes;
    // AvatarAura Aura;
    // AvatarType AvatarType;
    // AvatarChakras Chakras;
    // string Country;
    // string County;
    // OASISType CreatedOASISType;
    // mapping(DimensionLevel => string) DimensionLevelIds;
    // uint256 DOB;
    // Color FavouriteColour;
    // GeneKey GeneKeys;
    // AvatarGift Gifts;
    // HeartRateEntry HeartRateData;
    // HumanDesign HumanDesign;
    // string Image2D;
    // InventoryItem[] Inventory;
    // int Karma;
    // KarmaAkashicRecord[] KarmaAkashicRecords;
    // string Landline;
    // int Level;
    // string Mobile;
    // string Model3D;
    // Omiverse Omiverse;
    // string Postcode;
    // mapping(ProviderType => string) ProviderPrivateKey;
    // mapping(ProviderType => string) ProviderPublicKey;
    // mapping(ProviderType => string) ProviderUsername;
    // mapping(ProviderType => string) ProviderWalletAddress;
    // AvatarSkills Skills;
    // Spell[] Spells;
    // Color STARCLIColour;
    // AvatarStats Stats;
    // AvatarSkills SuperPowers;
    // string Town;
    // string UmaJson;
    // int XP;
}
