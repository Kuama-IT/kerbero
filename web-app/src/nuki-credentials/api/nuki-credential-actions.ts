import {
  NukiCredentialDraftResponseDto,
  NukiCredentialDraftResponseDtoSchema,
  NukiCredentialListResponseDto,
  NukiCredentialListResponseDtoSchema,
  NukiCredentialResponseDto,
  NukiCredentialResponseDtoSchema,
} from "./nuki-credential.schemas";
import { httpClient } from "../../shared/http-client/http-client";

export const startNukiAuthenticationFlow =
  async (): Promise<NukiCredentialDraftResponseDto> => {
    const json = await httpClient.post({
      endpoint: "/nuki-credentials/draft",
    });

    return NukiCredentialDraftResponseDtoSchema.parse(json);
  };

export const listNukiCredentials =
  async (): Promise<NukiCredentialListResponseDto> => {
    const json = await httpClient.get({ endpoint: "/nuki-credentials" });

    return NukiCredentialListResponseDtoSchema.parse(json);
  };

export const deleteNukiCredentials = async (
  nukiCredentialId: number
): Promise<NukiCredentialResponseDto> => {
  const json = await httpClient.delete({
    endpoint: `/api/nuki-credentials/${nukiCredentialId}`,
  });
  return NukiCredentialResponseDtoSchema.parse(json);
};
