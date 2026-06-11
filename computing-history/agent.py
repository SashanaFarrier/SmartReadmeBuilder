# Before running the sample:
#    pip install azure-ai-projects>=2.1.0

from azure.identity import DefaultAzureCredential
from azure.ai.projects import AIProjectClient

endpoint = "https://ms-ai-skills-fest-proj-resource.services.ai.azure.com/api/projects/ms-ai-skills-fest-proj"

project_client = AIProjectClient(
    endpoint=endpoint,
    credential=DefaultAzureCredential(),
)

my_agent = "computing-historian"
my_version = "1"

openai_client = project_client.get_openai_client()


def send_prompt(prompt: str) -> None:
    response = openai_client.responses.create(
        input=[{"role": "user", "content": prompt}],
        extra_body={"agent_reference": {"name": my_agent, "version": my_version, "type": "agent_reference"}},
    )

    print(f"\nResponse output: {response.output_text}")


def main() -> None:
    while True:
        prompt = input("Enter a prompt for the agent (or 'quit' to exit): ").strip()

        if not prompt:
            print("Please enter a prompt.")
            continue

        if prompt.lower() == "quit":
            print("Exiting agent session.")
            break

        send_prompt(prompt)


if __name__ == "__main__":
    main()



