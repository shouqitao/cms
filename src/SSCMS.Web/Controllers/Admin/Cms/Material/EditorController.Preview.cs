﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Enums;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Material
{
    public partial class EditorController
    {
        [HttpPost, Route(RouteActionsPreview)]
        public async Task<ActionResult<PreviewResult>> Preview([FromBody] PreviewRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                AuthTypes.SitePermissions.MaterialMessage))
            {
                return Unauthorized();
            }

            var (success, token, errorMessage) = await _wxManager.GetAccessTokenAsync(request.SiteId);
            if (success)
            {
                var mediaId = await _wxManager.PushMaterialAsync(token, MaterialType.Message, request.MessageId, false);

                foreach (var wxName in ListUtils.GetStringList(request.WxNames, Constants.Newline))
                {
                    await _wxManager.PreviewSendAsync(token, MaterialType.Message, mediaId, wxName);
                }
            }

            return new PreviewResult
            {
                Success = success,
                ErrorMessage = errorMessage
            };
        }
    }
}
