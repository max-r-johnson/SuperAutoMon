using Godot;
using GodotPlugins.Game;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

public partial class Pack
{
	public string name;
	public PetList petList;
	public FoodList foodList;
	public PetAbility PetAbility;
	public FoodAbility FoodAbility;
	public Pet pet;

	public Pack(string Name, PetList petlist, FoodList foodlist)
	{
		name = Name;
		this.petList = petlist;
		this.foodList = foodlist;
	}

	public Pack()
	{
	}

	public static Pack generateKanto()
	{
		//the concatenation should be done in generatePets() so that can get a specific tier
		PetList kantoPetList = new PetList();
		kantoPetList.tiers.Add(new List<Type>());
		kantoPetList.tiers.Add(new List<Type>{typeof(BulbasaurAbility), typeof(CharmanderAbility), typeof(SquirtleAbility), typeof(CaterpieAbility), typeof(WeedleAbility), typeof(PidgeyAbility), typeof(RattataAbility), typeof(SpearowAbility), typeof(EkansAbility), typeof(NidoranmAbility), typeof(NidoranfAbility)});
		kantoPetList.tiers.Add(new List<Type>{typeof(PikachuAbility), typeof(OddishAbility), typeof(ClefairyAbility), typeof(JigglypuffAbility), typeof(AbraAbility), typeof(ZubatAbility), typeof(GastlyAbility), typeof(BellsproutAbility), typeof(SandshrewAbility), typeof(MankeyAbility), typeof(PoliwagAbility), typeof(MeowthAbility)});
		kantoPetList.tiers.Add(new List<Type>{typeof(MachopAbility), typeof(MagikarpAbility), typeof(VulpixAbility), typeof(ParasAbility), typeof(VenonatAbility), typeof(DiglettAbility), typeof(PsyduckAbility), typeof(GeodudeAbility), typeof(OnixAbility), typeof(StaryuAbility), typeof(DrowzeeAbility), typeof(GoldeenAbility), typeof(GrowlitheAbility)});
		kantoPetList.tiers.Add(new List<Type>{typeof(TentacoolAbility), typeof(PonytaAbility), typeof(SlowpokeAbility), typeof(MagnemiteAbility), typeof(DoduoAbility), typeof(SeelAbility), typeof(GrimerAbility), typeof(KoffingAbility), typeof(HorseaAbility), typeof(KrabbyAbility), typeof(CuboneAbility), typeof(ExeggcuteAbility), typeof(VoltorbAbility), typeof(ShellderAbility), typeof(VenonatAbility)});
		kantoPetList.tiers.Add(new List<Type>{typeof(TaurosAbility), typeof(PinsirAbility), typeof(MagmarAbility), typeof(ElectabuzzAbility), typeof(JynxAbility), typeof(ScytherAbility), typeof(MrMimeAbility), typeof(FarfetchdAbility), typeof(LickitungAbility), typeof(RhyhornAbility), typeof(TangelaAbility), typeof(EeveeAbility)});
		kantoPetList.tiers.Add(new List<Type>{typeof(DratiniAbility), typeof(SnorlaxAbility), typeof(AerodactylAbility), typeof(OmanyteAbility), typeof(KabutoAbility), typeof(LaprasAbility), typeof(KangaskhanAbility), typeof(HitmonleeAbility), typeof(HitmonchanAbility), typeof(ChanseyAbility), typeof(DittoAbility), typeof(PorygonAbility)});
		kantoPetList.tiers.Add(new List<Type>{typeof(ArticunoAbility), typeof(ZapdosAbility), typeof(MoltresAbility), typeof(MewAbility), typeof(MewtwoAbility)});
		kantoPetList.generatePets();
		
		//for every pet in tier one of kanto, print the type of every pet's ability (weedleability, rattataability, etc.)

		FoodList kantoFoodList = new FoodList();
		kantoFoodList.tiers.Add(new List<Type>());
		kantoFoodList.tiers.Add(new List<Type>{typeof(TinyAppleAbility),typeof(OranBerryAbility)});
		kantoFoodList.tiers.Add(new List<Type>{typeof(DoomSeedAbility),typeof(GummiAbility),typeof(EnergyPowderAbility)});
		kantoFoodList.tiers.Add(new List<Type>{typeof(FreshWaterAbility),typeof(LeekAbility),typeof(BerryJuiceAbility)});
		kantoFoodList.tiers.Add(new List<Type>{typeof(EvolutionStoneAbility), typeof(SitrusBerryAbility), typeof(LumBerryAbility)});
		kantoFoodList.tiers.Add(new List<Type>{typeof(LemonadeAbility), typeof(RareCandyAbility), typeof(EjectButtonAbility)});
		kantoFoodList.tiers.Add(new List<Type>{typeof(ShellBellAbility), typeof(LeftoversAbility), typeof(EnergyRootAbility)});
		kantoFoodList.tiers.Add(new List<Type>());
		kantoFoodList.generateFoods();
		Pack Kanto = new Pack("Kanto",kantoPetList,kantoFoodList);

		// GD.Print(kantoPetList.ToString());
		// GD.Print(kantoFoodList.ToString());
		return Kanto;
	}

}