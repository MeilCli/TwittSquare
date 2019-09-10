using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreTweet;

namespace TwittSquare.Core.Utils.Extensions {
    public static class TextPartExtension {

        public static string ToText(this IEnumerable<TextPart> textParts) {
            var sb = new StringBuilder();
            foreach(var textPart in textParts) {
                if(textPart.Type == TextPartType.Url) {
                    sb.Append(((UrlEntity)textPart.Entity).ExpandedUrl);
                }else {
                    sb.Append(textPart.Text);
                }
            }
            return sb.ToString();
        }
    }
}
