using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AI.TextToImage;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalAppointment_KernalAI.Plugins
{
    public class SickNotesPlugin
    {
        private static string TemplateDirectory => AppDomain.CurrentDomain.BaseDirectory;

        private readonly ITextToImageService _textToImageService;

        public SickNotesPlugin(ITextToImageService textToImageService)
        {
            _textToImageService = textToImageService;
        }

        [KernelFunction]
        [Description("After completing the entire patient care process, understanding the symptoms and being sure of the patient's illness, I will generate a sick note.")]
        public async Task GenerateSickNoteAsync(
            Kernel kernel,
            [Description("Patient name")] string patientName,
            [Description("Patient age")] int patientAge,
            [Description("Patient Gender")] string patientGender,
            [Description("Doctor name")] string doctorName,
            [Description("Doctor hospital country")] string doctorHospitalCountry,
            [Description("Doctor hospital city")] string doctorHospitalCity,
            [Description("Symptoms presented by the patient")] string patientSymptoms,
            [Description("Possible illness of the patient")] string patientPossibleIllness)
        {
            var text = await GetTextAsync(kernel,
                patientName: patientName,
                patientSymptoms: patientSymptoms,
                patientPossibleIllness: patientPossibleIllness);


            SickNotesContentPatient patient = new(patientName, patientGender, patientAge);
            SickNotesContentDoctor doctor = new(doctorName, new(doctorHospitalCountry, doctorHospitalCity));

            SickNotesContent content = new(patient, doctor, DateTime.Today, text!);

            SickNotesTemplateGenerator.GenerateFile(TemplateDirectory, content);
        }

        async Task<string> GetTextAsync(Kernel kernel, string patientName, string patientSymptoms, string patientPossibleIllness)
        {
            var result = await kernel.InvokePromptAsync(
                @"
                My name is Jam and I am an experienced doctor with over 50 years of experience as a general practitioner. I work to improve the lives of my patients.
                Write text for a medical certificate for {{$patientName}}.

                The text for the medical certificate must be written taking into account the symptoms {{$patientSymptoms}} and the possible illness {{$patientPossibleIllness}}

                The text of the medical certificate must have a maximum of four lines and a minimum of two..

                The text must indicate how many days the patient will rest. To calculate how many days of rest the patient will need, it will be necessary to take into account the symptoms {{$patientSymptoms}} and the possible illness {{$patientPossibleIllness}}.
                    
                The text could follow the example ""Please excuse Vitor from the work on Monday and Tuesday, two days. It appears as though a serious case of winter fever and throat infection and is not yet been cured, I am prescribing two days complete bed rest with plenty of intake of liquid and oil free food along with proper dosage of the prescribed medicines.""
                ",
                new()
                {
                    { "patientName", patientName },
                    { "patientSymptoms", patientSymptoms },
                    { "patientPossibleIllness", patientPossibleIllness },
                });

            return result.GetValue<string>()!;
        }

        async Task<string> GetDoctorSignatureUrlAsync(Kernel kernel, string doctorName)
        {
            var result = await _textToImageService.GenerateImageAsync(
                $@"Generate signature text for the name {doctorName}.

It is to simulate the signature on a document.

The signature must be made with black font and a transparent background.

The background is completely transparent so that the signature is evident.

The signature only needs to have the person's name and nothing else. For example, if the person's name is ""Vitor"", the signature must only read ""Vitor""", 512, 512);

            return result;
        }
    }

    internal static class SickNotesTemplateGenerator
    {
        static string Generate(SickNotesContent content)
        {
            return $"<p style=\"margin-top:5pt; margin-bottom:5pt; text-align:center; line-height:18.15pt; background-color:#ffffff;\"><strong><em><span style=\"font-family:Helvetica; font-size:12pt;\">A Sample Doctor&rsquo;s Note for Work</span></em></strong></p>\r\n<p style=\"margin-top:5pt; margin-bottom:5pt; text-align:center; line-height:18.15pt; background-color:#ffffff;\"><strong><em><span style=\"font-family:Helvetica; font-size:12pt;\">&nbsp;</span></em></strong></p>\r\n<p style=\"margin-top:5pt; margin-bottom:5pt; text-align:center; line-height:18.15pt; background-color:#ffffff;\"><strong><span style=\"font-family:Helvetica; font-size:12pt;\">&nbsp;</span></strong></p>\r\n<p style=\"margin-top:5pt; margin-bottom:5pt; line-height:18.15pt; background-color:#ffffff;\"><span style=\"font-family:Constantia; font-size:12pt;\">ABC Medical Center</span></p>\r\n<p style=\"margin-top:5pt; margin-bottom:5pt; line-height:18.15pt; background-color:#ffffff;\"><span style=\"font-family:Constantia; font-size:12pt;\">{content.Doctor.Hospital.City}/{content.Doctor.Hospital.Country}</span></p>\r\n<p style=\"margin-top:5pt; margin-bottom:5pt; line-height:18.15pt; background-color:#ffffff;\"><span style=\"font-family:Constantia; font-size:12pt;\">Name: {content.Patient.Name}</span><span style=\"font-family:Constantia; font-size:12pt;\">&nbsp;</span><span style=\"font-family:Constantia; font-size:12pt;\">&nbsp;</span><span style=\"font-family:Constantia; font-size:12pt;\">&nbsp;</span><span style=\"font-family:Constantia; font-size:12pt;\">&nbsp;</span><span style=\"font-family:Constantia; font-size:12pt;\">&nbsp;</span><span style=\"font-family:Constantia; font-size:12pt;\">&nbsp;</span><span style=\"font-family:Constantia; font-size:12pt;\">&nbsp;</span><span style=\"font-family:Constantia; font-size:12pt;\">&nbsp;</span><span style=\"font-family:Constantia; font-size:12pt;\">&nbsp;</span><span style=\"font-family:Constantia; font-size:12pt;\">&nbsp;</span><span style=\"font-family:Constantia; font-size:12pt;\">&nbsp;</span><span style=\"font-family:Constantia; font-size:12pt;\">&nbsp;</span><span style=\"font-family:Constantia; font-size:12pt;\">&nbsp;</span><span style=\"font-family:Constantia; font-size:12pt;\">&nbsp;</span><span style=\"font-family:Constantia; font-size:12pt;\">&nbsp;Gender: {content.Patient.Gender}</span><span style=\"font-family:Constantia; font-size:12pt;\">&nbsp;</span><span style=\"font-family:Constantia; font-size:12pt;\">&nbsp;</span><span style=\"font-family:Constantia; font-size:12pt;\">&nbsp;</span><span style=\"font-family:Constantia; font-size:12pt;\">&nbsp;</span><span style=\"font-family:Constantia; font-size:12pt;\">&nbsp;</span><span style=\"font-family:Constantia; font-size:12pt;\">&nbsp;</span><span style=\"font-family:Constantia; font-size:12pt;\">&nbsp;</span><span style=\"font-family:Constantia; font-size:12pt;\">&nbsp;</span><span style=\"font-family:Constantia; font-size:12pt;\">&nbsp;</span><span style=\"font-family:Constantia; font-size:12pt;\">&nbsp;</span><span style=\"font-family:Constantia; font-size:12pt;\">&nbsp;Age: {content.Patient.Age}</span><span style=\"font-family:Constantia; font-size:12pt;\">&nbsp;</span><span style=\"font-family:Constantia; font-size:12pt;\">&nbsp;</span><span style=\"font-family:Constantia; font-size:12pt;\">&nbsp;</span><span style=\"font-family:Constantia; font-size:12pt;\">&nbsp;</span><span style=\"font-family:Constantia; font-size:12pt;\">&nbsp;</span><span style=\"font-family:Constantia; font-size:12pt;\">&nbsp;</span><span style=\"font-family:Constantia; font-size:12pt;\">&nbsp;</span><span style=\"font-family:Constantia; font-size:12pt;\">&nbsp;Date: {content.Date}</span></p>\r\n<p style=\"margin-top:5pt; margin-bottom:5pt; line-height:18.15pt; background-color:#ffffff;\"><span style=\"font-family:Constantia; font-size:12pt;\">{content.Text}</span><span style=\"font-family:Constantia; font-size:12pt;\">&nbsp;</span><span style=\"font-family:Constantia; font-size:12pt;\">prescribed</span><span style=\"font-family:Constantia; font-size:12pt;\">&nbsp;</span><span style=\"font-family:Constantia; font-size:12pt;\">medicines.</span></p>\r\n<p style=\"margin-top:5pt; margin-bottom:5pt; line-height:18.15pt; background-color:#ffffff;\"><span style=\"font-family:Constantia; font-size:12pt;\">Sincerely</span></p>\r\n<p style=\"margin-top:5pt; margin-bottom:5pt; line-height:18.15pt; background-color:#ffffff;\"><span style=\"font-family:Constantia; font-size:12pt;\">{content.Doctor.Name} (Signature of the doctor).</span></p>\r\n<p style=\"margin-top:0pt; margin-bottom:10pt; line-height:115%; font-size:12pt;\">&nbsp;</p>\r\n<p style=\"margin-top:0pt; margin-bottom:10pt; line-height:115%; font-size:12pt;\">&nbsp;</p>\r\n<p style=\"margin-top:0pt; margin-bottom:10pt; line-height:115%; font-size:12pt;\">&nbsp;</p>\r\n<p style=\"margin-top:0pt; margin-bottom:10pt; line-height:115%; font-size:12pt;\">&nbsp;</p>\r\n<p style=\"margin-top:0pt; margin-bottom:10pt; line-height:115%; font-size:12pt;\">&nbsp;</p>\r\n<p style=\"margin-top:0pt; margin-bottom:10pt; line-height:115%; font-size:12pt;\">&nbsp;</p>\r\n<p style=\"margin-top:0pt; margin-bottom:10pt; line-height:115%; font-size:12pt;\">&nbsp;</p>\r\n<p style=\"margin-top:0pt; margin-bottom:10pt; line-height:115%; font-size:12pt;\">&nbsp;</p>\r\n<p style=\"margin-top:0pt; margin-bottom:10pt; line-height:115%; font-size:12pt;\">&nbsp;</p>\r\n<p style=\"margin-top:0pt; margin-bottom:10pt; line-height:115%; font-size:12pt;\">&nbsp;</p>\r\n<p style=\"margin-top:0pt; margin-bottom:10pt; line-height:115%; font-size:12pt;\">&nbsp;</p>\r\n<p style=\"margin-top:0pt; margin-bottom:10pt; line-height:115%; font-size:12pt;\">&nbsp;</p>\r\n<p style=\"margin-top:0pt; margin-bottom:10pt; line-height:115%; font-size:12pt;\">&nbsp;</p>\r\n<p style=\"margin-top:0pt; margin-bottom:10pt; line-height:115%; font-size:12pt;\">&nbsp;</p>\r\n<p style=\"bottom: 10px; right: 10px; position: absolute;\">";
        }

        static string GenerateRandomFileName() => Path.GetRandomFileName() + ".html";

        internal static void GenerateFile(string directoryPath, SickNotesContent content)
        {
            var template = Generate(content);
            var filePath = Path.Combine(directoryPath, GenerateRandomFileName());

            File.WriteAllText(filePath, template);
        }
    }

    internal record SickNotesContentPatient(string Name, string Gender, int Age);

    internal record SickNotesContentDoctor(string Name, SickNotesContentDoctorHospital Hospital);

    internal record SickNotesContentDoctorHospital(string Country, string City);

    internal record SickNotesContent(SickNotesContentPatient Patient, SickNotesContentDoctor Doctor, DateTime Date, string Text);
}
