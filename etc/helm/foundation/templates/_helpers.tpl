{{- define "foundation.hosts.httpapi" -}}
{{- print "https://" (.Values.global.hosts.httpapi | replace "[RELEASE_NAME]" .Release.Name) -}}
{{- end -}}
{{- define "foundation.hosts.blazor" -}}
{{- print "https://" (.Values.global.hosts.blazor | replace "[RELEASE_NAME]" .Release.Name) -}}
{{- end -}}
{{- define "foundation.hosts.authserver" -}}
{{- print "https://" (.Values.global.hosts.authserver | replace "[RELEASE_NAME]" .Release.Name) -}}
{{- end -}}
