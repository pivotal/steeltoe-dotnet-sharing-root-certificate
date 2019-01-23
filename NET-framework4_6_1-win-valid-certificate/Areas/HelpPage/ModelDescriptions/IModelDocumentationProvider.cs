using System;
using System.Reflection;

namespace NET_framework4_6_1_win_valid_certificate.Areas.HelpPage.ModelDescriptions
{
    public interface IModelDocumentationProvider
    {
        string GetDocumentation(MemberInfo member);

        string GetDocumentation(Type type);
    }
}