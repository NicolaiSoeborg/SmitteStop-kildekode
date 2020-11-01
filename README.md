# SmitteStop

Teknologi kan spille en vigtig rolle i bekæmpelse af Covid-19.
Derfor er det meget uheldig at kritiske applikationer som “Smitte|Stop” app'en ikke er open-source.

Vi som borgere kan ikke blindt stole på software fra regeringen, eller private firmaer (Netcompany), faktisk burde vi antage at alt software kan være fejlbehæftet og have det i tankerne når vi træffer valg baseret på data fra software (_teleskandalen host host_).

Dette dokument og git repositorium er et forsøg på at åbne op for hvad app'en egentlig laver, hvordan smitteopsporingen udregnes, hvad det betyder når appen opdateres med release notes "[optimering af kontakopspring](https://github.com/NicolaiSoeborg/SmitteStop/compare/v1.1-99...v1.2-122)", osv, osv.

Bemærk at intet af denne viden gør app'en usikker og alt information er skaffet gennem _reverse engineering_ af den offentlige app.
Bare rolig Jens Rohde, ingen _net kommunisterne hacker_ har været på spil denne gang.

## Repo opbygning

Du kan benytte dig af alverdens git redskaber til at sammenligne ændringer på tværs af versioner.

Jeg vil foreslå at kigge på [ændringer på tværs af tags](https://github.com/nicolaisoeborg/smittestop/compare/).

## Android reversing 101

Først lidt info om app'en:

```
$ adb shell pm list packages | grep smittestop | cut -d':' -f2
com.netcompany.smittestop_exposure_notification

$ adb shell pm dump com.netcompany.smittestop_exposure_notification | grep -A1 'versionCode'
versionCode=99 minSdk=23 targetSdk=29
versionName=1.1
# Note: `versionName` er hvad du ser i Play Store, mens versionCode er byggenummeret

$ adb shell pm path com.netcompany.smittestop_exposure_notification | cut -d':' -f2
/data/app/com.netcompany.smittestop_exposure_notification-CIdMZZrgi6lQzN4Kmfn-9w==/base.apk
/data/app/com.netcompany.smittestop_exposure_notification-CIdMZZrgi6lQzN4Kmfn-9w==/split_config.arm64_v8a.apk
# [...] I version 1.0 var det en enkelt apk, mens det senere blev ændret til en "split apk"
```

Vi kan nu `adb pull <sti>` og skille app'en ad med [jadx](https://github.com/skylot/jadx).

I v1.0 kunne man finde `.dll`'erne i `/resources/assemblies/`, mens de i split apk'erne er komprimeret med lz4 (header: `XALZ`).
Vi kan bruge [Xamarin_XALZ_decompress.py](https://github.com/x41sec/tools/blob/master/Mobile/Xamarin/Xamarin_XALZ_decompress.py) til at udpakke `.dll`'erne.

### Xamarin

Vi finder hurtig ud af at java koden ikke er interessant, det er bare en indpakning rundt om en Xamarin app.
Til at skille .NET ad kan vi bruge [ILSpy](https://github.com/icsharpcode/ILSpy) (`dotnet tool install ilspycmd -g`).

De `.cs` filer du ser i roden af dette repo, er produceret af `ilspycmd`.

## Kontakt

Du skal være mere en velkommen til at [åbne et issue](https://github.com/NicolaiSoeborg/SmitteStop/issues/new) hvis du har spørgsmål. Jeg kan ikke love at besvare alle.

Hvis Netcompany vil i kontakt med mig, så kan de skrive direkte til `netcompany-smittestop@xn--sb-lka.org`.
