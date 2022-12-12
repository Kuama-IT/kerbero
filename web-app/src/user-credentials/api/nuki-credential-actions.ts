import {
  NukiCredentialDraftResponseDto,
  NukiCredentialDraftResponseDtoSchema,
  NukiCredentialListResponseDto,
  NukiCredentialListResponseDtoSchema,
  NukiCredentialResponseDto,
  NukiCredentialResponseDtoSchema,
} from "./nuki-credential.schemas";
import { httpClient } from "../../shared/http-client/http-client";

export const startNukiAuthenticationFlowAction =
  async (): Promise<NukiCredentialDraftResponseDto> => {
    const json = await httpClient.post({
      endpoint: "/user-credentials/draft",
    });

    return NukiCredentialDraftResponseDtoSchema.parse(json);
  };

export const listNukiCredentialsAction =
  async (): Promise<NukiCredentialListResponseDto> => {
    const json = await httpClient.get({ endpoint: "/user-credentials" });

    return NukiCredentialListResponseDtoSchema.parse(json);
  };

export const deleteNukiCredentialsAction = async (
  nukiCredentialId: number
): Promise<NukiCredentialResponseDto> => {
  const json = await httpClient.delete({
    endpoint: `/api/nuki-credentials/${nukiCredentialId}`,
  });
  return NukiCredentialResponseDtoSchema.parse(json);
};
