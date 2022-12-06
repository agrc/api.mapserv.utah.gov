using Google.Api.Gax.ResourceNames;
using Google.Cloud.RecaptchaEnterprise.V1;
using Serilog;
using System.Configuration;
using WebAPI.Common.Abstractions;

namespace WebAPI.Dashboard.Commands.ReCaptcha
{
    public class RecaptchaResult
    {
        public readonly float Score;
        public readonly bool Valid;

        public RecaptchaResult(float score, bool valid)
        {
            Score = score;
            Valid = valid;
        }
    }

    public class ValidateAssessmentCommand : Command<RecaptchaResult>
    {
        private readonly string _token;
        private readonly string _action;

        public ValidateAssessmentCommand(string token, string action)
        {
            _token = token;
            _action = action;
        }
        public override string ToString()
        {
            return string.Format("ValidateAssessmentCommand, action: {0}", _action);
        }

        protected override void Execute()
        {
            var log = Log.ForContext<ValidateAssessmentCommand>();
            var client = RecaptchaEnterpriseServiceClient.Create();

            var projectName = new ProjectName("ut-dts-agrc-recaptcha");

            var createAssessmentRequest = new CreateAssessmentRequest()
            {
                Assessment = new Assessment()
                {
                    Event = new Event()
                    {
                        SiteKey = ConfigurationManager.AppSettings["recaptcha_key"],
                        Token = _token,
                        ExpectedAction = _action
                    },
                },
                ParentAsProjectName = projectName
            };

            var response = client.CreateAssessment(createAssessmentRequest);

            if (response.TokenProperties.Valid == false)
            {
                log.Warning("The CreateAssessment call failed because the token was: {error}",
                    response.TokenProperties.InvalidReason.ToString());

                Result = new RecaptchaResult(-1f, false);
                return;
            }

            if (response.TokenProperties.Action != _action)
            {
                log.Warning("The action attribute in the reCAPTCHA tag does not " +
                    "match the action you are expecting to score {expected} != {actual}", _action, response.TokenProperties.Action.ToString());
                
                Result = new RecaptchaResult(-1, false);

                return;
            }

            var score = response.RiskAnalysis.Score;

            foreach(var reason in response.RiskAnalysis.Reasons)
            {
                log.Information("recaptcha scoring reasons {reason}", reason);
            }

            Result = new RecaptchaResult(score, score >= .5);
        }
    }
}