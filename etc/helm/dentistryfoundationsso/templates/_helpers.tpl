{{- define "dentistryfoundationsso.hosts.httpapi" -}}
{{- print "https://" (.Values.global.hosts.httpapi | replace "[RELEASE_NAME]" .Release.Name) -}}
{{- end -}}
{{- define "dentistryfoundationsso.hosts.blazor" -}}
{{- print "https://" (.Values.global.hosts.blazor | replace "[RELEASE_NAME]" .Release.Name) -}}
{{- end -}}
{{- define "dentistryfoundationsso.hosts.authserver" -}}
{{- print "https://" (.Values.global.hosts.authserver | replace "[RELEASE_NAME]" .Release.Name) -}}
{{- end -}}
