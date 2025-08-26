using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Domain.Enums
{
    public enum EgyptCities
    {
        // الدقهلية - AD_DAQAHLIYAH
        [Description("المنصورة")]
        MANSOURA_DAQAHLIYAH,
        [Description("طلخا")]
        TALKHA_DAQAHLIYAH,
        [Description("المطرية")]
        MIT_GHAMR_DAQAHLIYAH,
        [Description("دكرنس")]
        DIKIRNIS_DAQAHLIYAH,
        [Description("أجا")]
        AGA_DAQAHLIYAH,
        [Description("منية النصر")]
        MINYAT_AL_NASR_DAQAHLIYAH,
        [Description("السنبلاوين")]
        SINBILLAWIN_DAQAHLIYAH,
        [Description("تمي الأمديد")]
        TAMI_AL_AMDID_DAQAHLIYAH,

        // البحيرة - AL_BUHAYRAH
        [Description("دمنهور")]
        DAMANHUR_BUHAYRAH,
        [Description("كفر الدوار")]
        KAFR_AL_DAWWAR_BUHAYRAH,
        [Description("رشيد")]
        RASHID_BUHAYRAH,
        [Description("إدكو")]
        IDKU_BUHAYRAH,
        [Description("أبو المطامير")]
        ABU_AL_MATAMIR_BUHAYRAH,
        [Description("حوش عيسى")]
        HOUSH_EISSA_BUHAYRAH,
        [Description("شبراخيت")]
        SHUBRAKHIT_BUHAYRAH,
        [Description("كوم حمادة")]
        KOM_HAMADA_BUHAYRAH,

        // الغربية - AL_GHARBIYAH
        [Description("طنطا")]
        TANTA_GHARBIYAH,
        [Description("المحلة الكبرى")]
        MAHALLA_AL_KUBRA_GHARBIYAH,
        [Description("كفر الزيات")]
        KAFR_AL_ZAYAT_GHARBIYAH,
        [Description("زفتى")]
        ZIFTA_GHARBIYAH,
        [Description("السنطة")]
        SANTA_GHARBIYAH,
        [Description("قطور")]
        QUTUR_GHARBIYAH,
        [Description("بسيون")]
        BASIYUN_GHARBIYAH,
        [Description("سمنود")]
        SAMANNUD_GHARBIYAH,

        // الإسكندرية - AL_ISKANDARIYAH
        [Description("الإسكندرية")]
        ALEXANDRIA_CENTER,
        [Description("المنتزة")]
        MONTAZA_ALEXANDRIA,
        [Description("العامرية")]
        AMRIA_ALEXANDRIA,
        [Description("برج العرب")]
        BORG_AL_ARAB_ALEXANDRIA,
        [Description("الدخيلة")]
        DEKHEILA_ALEXANDRIA,
        [Description("أبو قير")]
        ABU_QIR_ALEXANDRIA,
        [Description("الجمرك")]
        GOMROK_ALEXANDRIA,
        [Description("المعمورة")]
        MAMOURA_ALEXANDRIA,

        // الإسماعيلية - AL_ISMAILIA
        [Description("الإسماعيلية")]
        ISMAILIA_CENTER,
        [Description("فايد")]
        FAYED_ISMAILIA,
        [Description("القنطرة شرق")]
        QANTARA_SHARQ_ISMAILIA,
        [Description("القنطرة غرب")]
        QANTARA_GHARB_ISMAILIA,
        [Description("أبو صوير")]
        ABU_SUWIR_ISMAILIA,
        [Description("الكساسين")]
        KASSASEIN_ISMAILIA,
        [Description("التل الكبير")]
        TEL_AL_KABIR_ISMAILIA,

        // المنوفية - AL_MINUFIYAH
        [Description("شبين الكوم")]
        SHIBIN_AL_KOM_MINUFIYAH,
        [Description("منوف")]
        MINUF_MINUFIYAH,
        [Description("سرس الليان")]
        SIRS_AL_LAYAN_MINUFIYAH,
        [Description("أشمون")]
        ASHMUN_MINUFIYAH,
        [Description("الباجور")]
        BAGUR_MINUFIYAH,
        [Description("قويسنا")]
        QUWAYSINA_MINUFIYAH,
        [Description("تلا")]
        TALA_MINUFIYAH,
        [Description("الشهداء")]
        SHUHADA_MINUFIYAH,

        // القليوبية - AL_QALYUBIYAH
        [Description("بنها")]
        BENHA_QALYUBIYAH,
        [Description("شبرا الخيمة")]
        SHUBRA_AL_KHAYMAH_QALYUBIYAH,
        [Description("القناطر الخيرية")]
        QANATIR_AL_KHAYRIYAH_QALYUBIYAH,
        [Description("كفر شكر")]
        KAFR_SHUKR_QALYUBIYAH,
        [Description("طوخ")]
        TUKH_QALYUBIYAH,
        [Description("قليوب")]
        QALYUB_QALYUBIYAH,
        [Description("الخانكة")]
        KHANKA_QALYUBIYAH,
        [Description("شبين القناطر")]
        SHIBIN_AL_QANATIR_QALYUBIYAH,

        // الشرقية - ASH_SHARQIYAH
        [Description("الزقازيق")]
        ZAGAZIG_SHARQIYAH,
        [Description("العاشر من رمضان")]
        TENTH_OF_RAMADAN_SHARQIYAH,
        [Description("بلبيس")]
        BILBAYS_SHARQIYAH,
        [Description("مشتول السوق")]
        MISHTUL_AS_SUQ_SHARQIYAH,
        [Description("القرين")]
        QARIN_SHARQIYAH,
        [Description("أبو حماد")]
        ABU_HAMMAD_SHARQIYAH,
        [Description("فاقوس")]
        FAQUS_SHARQIYAH,
        [Description("كفر صقر")]
        KAFR_SAQR_SHARQIYAH,

        // دمياط - DUMYAT
        [Description("دمياط")]
        DAMIETTA_CENTER,
        [Description("رأس البر")]
        RAS_AL_BAR_DAMIETTA,
        [Description("فارسكور")]
        FARASKUR_DAMIETTA,
        [Description("الزرقا")]
        ZARQA_DAMIETTA,
        [Description("كفر سعد")]
        KAFR_SAAD_DAMIETTA,
        [Description("عزبة البرج")]
        EZBAT_AL_BORG_DAMIETTA,

        // كفر الشيخ - KAFR_ASH_SHAYKH
        [Description("كفر الشيخ")]
        KAFR_ASH_SHAYKH_CENTER,
        [Description("دسوق")]
        DISUQ_KAFR_ASH_SHAYKH,
        [Description("فوة")]
        FUWA_KAFR_ASH_SHAYKH,
        [Description("قلين")]
        QILIN_KAFR_ASH_SHAYKH,
        [Description("سيدي سالم")]
        SIDI_SALIM_KAFR_ASH_SHAYKH,
        [Description("بيلا")]
        BILA_KAFR_ASH_SHAYKH,
        [Description("الحامول")]
        HAMUL_KAFR_ASH_SHAYKH,
        [Description("بلطيم")]
        BALTIM_KAFR_ASH_SHAYKH,

        // الفيوم - AL_FAYYUM
        [Description("الفيوم")]
        FAYYUM_CENTER,
        [Description("سنورس")]
        SINNURIS_FAYYUM,
        [Description("طامية")]
        TAMIYA_FAYYUM,
        [Description("إطسا")]
        ITSA_FAYYUM,
        [Description("إبشواي")]
        IBSHAWAY_FAYYUM,
        [Description("يوسف الصديق")]
        YUSUF_AS_SIDDIQ_FAYYUM,

        // المنيا - AL_MINYA
        [Description("المنيا")]
        MINYA_CENTER,
        [Description("ملوي")]
        MALLAWI_MINYA,
        [Description("سمالوط")]
        SAMALUT_MINYA,
        [Description("المطيا")]
        MATAI_MINYA,
        [Description("بني مزار")]
        BANI_MAZAR_MINYA,
        [Description("مغاغة")]
        MAGHAGHA_MINYA,
        [Description("العدوة")]
        ADWA_MINYA,
        [Description("دير مواس")]
        DEIR_MAWAS_MINYA,

        // أسيوط - ASYUT
        [Description("أسيوط")]
        ASYUT_CENTER,
        [Description("ديروط")]
        DAYRUT_ASYUT,
        [Description("منفلوط")]
        MANFALUT_ASYUT,
        [Description("القوصية")]
        QUSIYA_ASYUT,
        [Description("أبنوب")]
        ABNUB_ASYUT,
        [Description("الفتح")]
        FATEH_ASYUT,
        [Description("ساحل سليم")]
        SAHEL_SELIM_ASYUT,
        [Description("البداري")]
        BADARI_ASYUT,

        // سوهاج - SUHAJ
        [Description("سوهاج")]
        SOHAG_CENTER,
        [Description("أخميم")]
        AKHMIM_SOHAG,
        [Description("البلينا")]
        BALYANA_SOHAG,
        [Description("المراغة")]
        MARAGHA_SOHAG,
        [Description("طما")]
        TAMA_SOHAG,
        [Description("طهطا")]
        TAHTA_SOHAG,
        [Description("جرجا")]
        GIRGA_SOHAG,
        [Description("العسيرات")]
        ASIRAT_SOHAG,

        // قنا - QINA
        [Description("قنا")]
        QENA_CENTER,
        [Description("الوقف")]
        WAQF_QENA,
        [Description("قفط")]
        QIFT_QENA,
        [Description("قوص")]
        QUS_QENA,
        [Description("نقادة")]
        NAQADA_QENA,
        [Description("دشنا")]
        DISHNA_QENA,
        [Description("فرشوط")]
        FARSHUT_QENA,
        [Description("نجع حمادي")]
        NAGA_HAMMADI_QENA,

        // الأقصر - AL_UQSUR
        [Description("الأقصر")]
        LUXOR_CENTER,
        [Description("إسنا")]
        ISNA_LUXOR,
        [Description("أرمنت")]
        ARMANT_LUXOR,
        [Description("القرنة")]
        QURNA_LUXOR,
        [Description("الطود")]
        TOD_LUXOR,

        // أسوان - ASWAN
        [Description("أسوان")]
        ASWAN_CENTER,
        [Description("إدفو")]
        IDFU_ASWAN,
        [Description("كوم أمبو")]
        KOM_OMBO_ASWAN,
        [Description("دراو")]
        DARAW_ASWAN,
        [Description("نصر النوبة")]
        NASR_AL_NUBA_ASWAN,
        [Description("أبو سمبل")]
        ABU_SIMBEL_ASWAN,

        // بني سويف - BANI_SUWAYF
        [Description("بني سويف")]
        BANI_SUEF_CENTER,
        [Description("الواسطى")]
        WASTA_BANI_SUEF,
        [Description("ناصر")]
        NASER_BANI_SUEF,
        [Description("إهناسيا")]
        IHNASYA_BANI_SUEF,
        [Description("ببا")]
        BIBA_BANI_SUEF,
        [Description("سمسطا")]
        SUMUSTA_BANI_SUEF,
        [Description("الفشن")]
        FASHN_BANI_SUEF,

        // القاهرة - AL_QAHIRAH
        [Description("وسط البلد")]
        DOWNTOWN_CAIRO,
        [Description("مصر الجديدة")]
        HELIOPOLIS_CAIRO,
        [Description("النزهة")]
        NUZHA_CAIRO,
        [Description("مدينة نصر")]
        NASR_CITY_CAIRO,
        [Description("المعادي")]
        MAADI_CAIRO,
        [Description("حلوان")]
        HELWAN_CAIRO,
        [Description("المطرية")]
        MATARIA_CAIRO,
        [Description("عين شمس")]
        AIN_SHAMS_CAIRO,
        [Description("الزيتون")]
        ZEITOUN_CAIRO,
        [Description("مصر القديمة")]
        OLD_CAIRO,
        [Description("البساتين")]
        BASATIN_CAIRO,
        [Description("السيدة زينب")]
        SAYEDA_ZEINAB_CAIRO,
        [Description("الخليفة")]
        KHALIFA_CAIRO,
        [Description("المقطم")]
        MUQATTAM_CAIRO,
        [Description("التجمع الخامس")]
        NEW_CAIRO_FIFTH_SETTLEMENT,

        // الجيزة - AL_JIZAH
        [Description("الجيزة")]
        GIZA_CENTER,
        [Description("6 أكتوبر")]
        SIXTH_OF_OCTOBER_GIZA,
        [Description("الشيخ زايد")]
        SHEIKH_ZAYED_GIZA,
        [Description("الدقي")]
        DOKKI_GIZA,
        [Description("المهندسين")]
        MOHANDESSIN_GIZA,
        [Description("العجوزة")]
        AGOUZA_GIZA,
        [Description("الهرم")]
        HARAM_GIZA,
        [Description("فيصل")]
        FAISAL_GIZA,
        [Description("العمرانية")]
        OMRANIA_GIZA,
        [Description("كرداسة")]
        KERDASA_GIZA,
        [Description("أوسيم")]
        AUSIM_GIZA,
        [Description("البدرشين")]
        BADRASHEIN_GIZA,
        [Description("الصف")]
        SAFF_GIZA,
        [Description("أطفيح")]
        ATFIH_GIZA,

        // بورسعيد - BUR_SAID
        [Description("بورسعيد")]
        PORT_SAID_CENTER,
        [Description("بور فؤاد")]
        PORT_FOUAD_PORT_SAID,
        [Description("العرب")]
        ARAB_PORT_SAID,
        [Description("الزهور")]
        ZOHOUR_PORT_SAID,
        [Description("المناخ")]
        MANAKH_PORT_SAID,

        // السويس - AS_SUWAYS
        [Description("السويس")]
        SUEZ_CENTER,
        [Description("الأربعين")]
        ARBAIN_SUEZ,
        [Description("عتاقة")]
        ATAQA_SUEZ,
        [Description("الجناين")]
        GANAYEN_SUEZ,
        [Description("فيصل")]
        FAISAL_SUEZ,

        // البحر الأحمر - AL_BAHR_AL_AHMAR
        [Description("الغردقة")]
        HURGHADA_RED_SEA,
        [Description("سفاجا")]
        SAFAGA_RED_SEA,
        [Description("القصير")]
        QUSEIR_RED_SEA,
        [Description("مرسى علم")]
        MARSA_ALAM_RED_SEA,
        [Description("رأس غارب")]
        RAS_GHAREB_RED_SEA,
        [Description("الشلاتين")]
        SHALATEEN_RED_SEA,
        [Description("حلايب")]
        HALAIB_RED_SEA,

        // الوادي الجديد - AL_WADI_AL_JADID
        [Description("الخارجة")]
        KHARGA_NEW_VALLEY,
        [Description("الداخلة")]
        DAKHLA_NEW_VALLEY,
        [Description("الفرافرة")]
        FARAFRA_NEW_VALLEY,
        [Description("باريس")]
        PARIS_NEW_VALLEY,
        [Description("بلاط")]
        BALAT_NEW_VALLEY,

        // مطروح - MATRUH
        [Description("مرسى مطروح")]
        MARSA_MATRUH_CENTER,
        [Description("العلمين")]
        EL_ALAMEIN_MATRUH,
        [Description("الحمام")]
        HAMMAM_MATRUH,
        [Description("الضبعة")]
        DABAA_MATRUH,
        [Description("النجيلة")]
        NEGILA_MATRUH,
        [Description("سيدي براني")]
        SIDI_BARANI_MATRUH,
        [Description("السلوم")]
        SALLOUM_MATRUH,

        // شمال سيناء - SHAMAL_SINA
        [Description("العريش")]
        ARISH_NORTH_SINAI,
        [Description("الشيخ زويد")]
        SHEIKH_ZUWEID_NORTH_SINAI,
        [Description("رفح")]
        RAFAH_NORTH_SINAI,
        [Description("بئر العبد")]
        BIR_AL_ABD_NORTH_SINAI,
        [Description("الحسنة")]
        HASANA_NORTH_SINAI,
        [Description("نخل")]
        NAKHL_NORTH_SINAI,

        // جنوب سيناء - JANUB_SINA
        [Description("الطور")]
        TUR_SOUTH_SINAI,
        [Description("شرم الشيخ")]
        SHARM_EL_SHEIKH_SOUTH_SINAI,
        [Description("دهب")]
        DAHAB_SOUTH_SINAI,
        [Description("نويبع")]
        NUWEIBA_SOUTH_SINAI,
        [Description("طابا")]
        TABA_SOUTH_SINAI,
        [Description("كاترين")]
        KATREEN_SOUTH_SINAI,
        [Description("رأس سدر")]
        RAS_SUDR_SOUTH_SINAI,
        [Description("أبو رديس")]
        ABU_RUDEIS_SOUTH_SINAI
    }
}